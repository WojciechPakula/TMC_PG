using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public abstract class GISlayer {
    private Dictionary<Vector3Int, Texture2D> segmentCache = new Dictionary<Vector3Int, Texture2D>();
    private Dictionary<Vector3Int, byte[]> segmentCacheByte = new Dictionary<Vector3Int, byte[]>();
    public float opacity = 1.0f;
    public abstract Texture2D renderSegment(int x, int y, int z);
    public abstract byte[] renderSegmentThreadSafe(int x, int y, int z);

    public Texture2D renderSegment(Vector3Int coords)
    {
        return renderSegment(coords.x, coords.y, coords.z);
    }
    public Texture2D renderSegmentWithCache(Vector3Int coords)
    {
        var c = getCacheSegment(coords.x,coords.y,coords.z);
        if (c==null)
        {
            var res = renderSegment(coords.x, coords.y, coords.z);
            if (res != null)
                setCacheSegment(coords.x, coords.y, coords.z, res);
            return res;
        }
        return c;       
    }
    public byte[] renderSegmentThreadSafe(Vector3Int coords)
    {
        return renderSegmentThreadSafe(coords.x, coords.y, coords.z);
    }
    public byte[] renderSegmentWithCacheThreadSafe(Vector3Int coords)
    {
        var c = getCacheSegmentByte(coords.x, coords.y, coords.z);
        if (c == null)
        {
            c = renderSegmentThreadSafe(coords.x, coords.y, coords.z);
            if (c != null)
            {
                setCacheSegmentByte(coords.x, coords.y, coords.z, c);               
            }
        }
        if (c != null)        
            for (int i = 0; i < c.Length; ++i)
            {
                c[i] = (byte)((float)c[i] * opacity);
            }              
        return c;
    }
    /*public float4 renderSegmentWithCacheThreadSafe(Vector3Int coords)
    {
        var c = getCacheSegment(coords.x, coords.y, coords.z);
        if (c == null)
        {
            var res = renderSegment(coords.x, coords.y, coords.z);
            setCacheSegment(coords.x, coords.y, coords.z, res);
            return res;
        }
        return c;
    }*/
    public Texture2D getCacheSegment(int x, int y, int z)
    {
        Texture2D result = null;
        bool val = segmentCache.TryGetValue(new Vector3Int(x,y,z), out result);
        if (val == false) return null;
        return result;
    }
    public byte[] getCacheSegmentByte(int x, int y, int z)
    {
        byte[] result = null;
        bool val = segmentCacheByte.TryGetValue(new Vector3Int(x, y, z), out result);
        if (val == false) return null;
        return result;
    }
    public void setCacheSegment(int x, int y, int z, Texture2D tex)
    {
        segmentCache.Add(new Vector3Int(x,y,z),tex);
    }
    public void setCacheSegment(Vector3Int coords, Texture2D tex)
    {
        setCacheSegment(coords.x, coords.y, coords.z,tex);
    }
    public void setCacheSegmentByte(int x, int y, int z, byte[] tex)
    {
        segmentCacheByte.Add(new Vector3Int(x, y, z), tex);
    }
    public void setCacheSegmentByte(Vector3Int coords, byte[] tex)
    {
        setCacheSegmentByte(coords.x, coords.y, coords.z, tex);
    }
    public void clearCache()
    {
        segmentCache.Clear();
    }
    /*{        
        Texture2D result = new Texture2D(256,256);
        Color32 resetColor = new Color32(0, 0, 0, 0);
        Color32[] resetColorArray = result.GetPixels32();
        for (int i = 0; i < resetColorArray.Length; i++)       
            resetColorArray[i] = resetColor;       
        result.SetPixels32(resetColorArray);


        result.Apply();
        return result;
    }*/

    //web
    public static Vector2d LatLonToWeb(Vector2d latlon, int z = 0)
    {
        Vector2d result;
        latlon *= Mathd.PI / 180.0;
        result.y = -System.Math.Log(System.Math.Tan(Mathd.PI / 4.0 + latlon.x / 2.0)) + Mathd.PI;
        result.x = latlon.y + Mathd.PI;

        result = result * ((256 / (2 * Mathd.PI)) * Mathd.Pow(2, z));        
        return result;
    }
    public static Vector2d WebToLatLon(Vector2d web, int z = 0)
    {
        Vector2d result;
        web = web / ((256 / (2 * Mathd.PI)) * Mathd.Pow(2, z));
        result.y = 2 * System.Math.Atan(System.Math.Exp(-web.x + Mathd.PI)) - Mathd.PI / 2.0;
        result.x = web.y - Mathd.PI;

        result *= 180.0 / Mathd.PI;
        return result;
    }
    //Unitary
    public static Vector2d LatLonToUnitary(Vector2d latlon)
    {
        Vector2d result= LatLonToWeb(latlon, 0);
        result.x /= 256.0;
        result.y /= 256.0;
        return result;
    }
    public static Vector2d UnitaryToLatLon(Vector2d unitary)
    {
        Vector2d result = WebToLatLon(new Vector2d(unitary.x*256, unitary.y*256), 0);
        return result;
    }
    //segment
    public static Vector3Int unitaryToSegment(Vector2d unitary, int zoom)
    {
        Vector3Int result = new Vector3Int((int)(unitary.x*(double)(1 << zoom)), (int)(unitary.y* (double)(1 << zoom)), zoom);
        return result;
    }
    public static Vector2d segmentToUnitary(Vector3Int segment)
    {
        Vector2d result = new Vector2d((double)segment.x/(double)(1<<segment.z), (double)segment.y / (double)(1 << segment.z));
        return result;
    }
    /*public static byte[] ImageToByte(Bitmap img)
    {
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(img, typeof(byte[]));
    }
    public static byte[] ImageToByte2(Image img)
    {
        using (var stream = new MemoryStream())
        {
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return stream.ToArray();
        }
    }*/
    /*public static byte[] getImageRasterBytes(Bitmap bmp, PixelFormat format)
    {
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        byte[] bits = null;

        try
        {
            // Lock the managed memory
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadWrite, format);

            // Declare an array to hold the bytes of the bitmap.
            bits = new byte[bmpdata.Stride * bmpdata.Height];

            // Copy the values into the array.
            System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, bits, 0, bits.Length);

            // Release managed memory
            bmp.UnlockBits(bmpdata);
        }
        catch
        {
            return null;
        }

        return bits;
    }*/
    public static byte[] bingPath(string path)
    {
        //Debug.Log("GISlayer start: "+ path);
        Bitmap bmp = new Bitmap(path);
        //Debug.Log("GISlayer bitmap");
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData =
           bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
        byte[] rgbValues = new byte[bytes];
        //Debug.Log("GISlayer copy");
        // Copy the RGB values into the array.
        Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

        byte[] rgbaValues = new byte[bmp.Width* bmp.Height*4];
        //Debug.Log("GISlayer mem");
        for (int x = 0; x < bmp.Width; x++)
        {
            for (int y = 0; y < bmp.Width; y++)
            {
                rgbaValues[(x + bmp.Width * y) * 4 + 0] = rgbValues[(x + bmp.Width * (y)) * 3 + 2];
                rgbaValues[(x + bmp.Width * y) * 4 + 1] = rgbValues[(x + bmp.Width * (y)) * 3 + 1];
                rgbaValues[(x + bmp.Width * y) * 4 + 2] = rgbValues[(x + bmp.Width * (y)) * 3 + 0];
                rgbaValues[(x + bmp.Width * y) * 4 + 3] = 255;
            }
        }
        /*byte[] argbTex= new byte[bmp.Width* bmp.Height* 4];
        for (int i = 0; i < rgbValues.Length; i+=3)
        {
            rgbTex[i];
        }*/
        //Debug.Log("GISlayer end");
        return rgbaValues;
    }
    public static byte[] osmPath(string path)
    {
        //Debug.Log("GISlayer start: " + path);
        Bitmap bmp = new Bitmap(path);

        //Bitmap bmpIn = (Bitmap)Bitmap.FromFile(inputFileName);

        Bitmap converted = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(converted))
        {
            // Prevent DPI conversion
            g.PageUnit = GraphicsUnit.Pixel;
            // Draw the image
            g.DrawImageUnscaled(bmp, 0, 0);
        }
        bmp = converted;
        //Debug.Log("GISlayer bitmap");
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData =
           bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
        byte[] rgbValues = new byte[bytes];
        //Debug.Log("GISlayer copy");
        // Copy the RGB values into the array.
        Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

        byte[] rgbaValues = new byte[bmp.Width * bmp.Height * 4];
        //Debug.Log("GISlayer mem");
        for (int x = 0; x < bmp.Width; x++)
        {
            for (int y = 0; y < bmp.Width; y++)
            {
                rgbaValues[(x + bmp.Width * y) * 4 + 0] = rgbValues[(x + bmp.Width * (y)) * 3 + 2];
                rgbaValues[(x + bmp.Width * y) * 4 + 1] = rgbValues[(x + bmp.Width * (y)) * 3 + 1];
                rgbaValues[(x + bmp.Width * y) * 4 + 2] = rgbValues[(x + bmp.Width * (y)) * 3 + 0];
                rgbaValues[(x + bmp.Width * y) * 4 + 3] = 255;
            }
        }
        /*byte[] argbTex= new byte[bmp.Width* bmp.Height* 4];
        for (int i = 0; i < rgbValues.Length; i+=3)
        {
            rgbTex[i];
        }*/
        //Debug.Log("GISlayer end");
        return rgbaValues;
    }
    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;
        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }
        return isOk;
    }
}
