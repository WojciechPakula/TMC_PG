using System.Xml.Serialization;

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class tag {
    
    private string kField;
    
    private string vField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string k {
        get {
            return this.kField;
        }
        set {
            this.kField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string v {
        get {
            return this.vField;
        }
        set {
            this.vField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class osm {
    
    private osmBounds[] boundsField;
    
    private osmNode[] nodeField;
    
    private osmWay[] wayField;
    
    private osmRelation[] relationField;
    
    private string versionField;
    
    private string generatorField;
    
    private string copyrightField;
    
    private string attributionField;
    
    private string licenseField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("bounds", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmBounds[] bounds {
        get {
            return this.boundsField;
        }
        set {
            this.boundsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("node", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmNode[] node {
        get {
            return this.nodeField;
        }
        set {
            this.nodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("way", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmWay[] way {
        get {
            return this.wayField;
        }
        set {
            this.wayField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("relation", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmRelation[] relation {
        get {
            return this.relationField;
        }
        set {
            this.relationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string generator {
        get {
            return this.generatorField;
        }
        set {
            this.generatorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string copyright {
        get {
            return this.copyrightField;
        }
        set {
            this.copyrightField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string attribution {
        get {
            return this.attributionField;
        }
        set {
            this.attributionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string license {
        get {
            return this.licenseField;
        }
        set {
            this.licenseField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmBounds {
    
    private string minlatField;
    
    private string minlonField;
    
    private string maxlatField;
    
    private string maxlonField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string minlat {
        get {
            return this.minlatField;
        }
        set {
            this.minlatField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string minlon {
        get {
            return this.minlonField;
        }
        set {
            this.minlonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string maxlat {
        get {
            return this.maxlatField;
        }
        set {
            this.maxlatField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string maxlon {
        get {
            return this.maxlonField;
        }
        set {
            this.maxlonField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmNode {
    
    private tag[] tagField;
    
    private string idField;
    
    private string visibleField;
    
    private string versionField;
    
    private string changesetField;
    
    private string timestampField;
    
    private string userField;
    
    private string uidField;
    
    private string latField;
    
    private string lonField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("tag")]
    public tag[] tag {
        get {
            return this.tagField;
        }
        set {
            this.tagField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string visible {
        get {
            return this.visibleField;
        }
        set {
            this.visibleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string changeset {
        get {
            return this.changesetField;
        }
        set {
            this.changesetField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string timestamp {
        get {
            return this.timestampField;
        }
        set {
            this.timestampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string user {
        get {
            return this.userField;
        }
        set {
            this.userField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string uid {
        get {
            return this.uidField;
        }
        set {
            this.uidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string lat {
        get {
            return this.latField;
        }
        set {
            this.latField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string lon {
        get {
            return this.lonField;
        }
        set {
            this.lonField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmWay {
    
    private osmWayND[] ndField;
    
    private tag[] tagField;
    
    private string idField;
    
    private string visibleField;
    
    private string versionField;
    
    private string changesetField;
    
    private string timestampField;
    
    private string userField;
    
    private string uidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("nd", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmWayND[] nd {
        get {
            return this.ndField;
        }
        set {
            this.ndField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("tag")]
    public tag[] tag {
        get {
            return this.tagField;
        }
        set {
            this.tagField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string visible {
        get {
            return this.visibleField;
        }
        set {
            this.visibleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string changeset {
        get {
            return this.changesetField;
        }
        set {
            this.changesetField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string timestamp {
        get {
            return this.timestampField;
        }
        set {
            this.timestampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string user {
        get {
            return this.userField;
        }
        set {
            this.userField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string uid {
        get {
            return this.uidField;
        }
        set {
            this.uidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmWayND {
    
    private string refField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string @ref {
        get {
            return this.refField;
        }
        set {
            this.refField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmRelation {
    
    private osmRelationMember[] memberField;
    
    private tag[] tagField;
    
    private string idField;
    
    private string visibleField;
    
    private string versionField;
    
    private string changesetField;
    
    private string timestampField;
    
    private string userField;
    
    private string uidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("member", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public osmRelationMember[] member {
        get {
            return this.memberField;
        }
        set {
            this.memberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("tag")]
    public tag[] tag {
        get {
            return this.tagField;
        }
        set {
            this.tagField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string visible {
        get {
            return this.visibleField;
        }
        set {
            this.visibleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string changeset {
        get {
            return this.changesetField;
        }
        set {
            this.changesetField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string timestamp {
        get {
            return this.timestampField;
        }
        set {
            this.timestampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string user {
        get {
            return this.userField;
        }
        set {
            this.userField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string uid {
        get {
            return this.uidField;
        }
        set {
            this.uidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class osmRelationMember {
    
    private string typeField;
    
    private string refField;
    
    private string roleField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string @ref {
        get {
            return this.refField;
        }
        set {
            this.refField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string role {
        get {
            return this.roleField;
        }
        set {
            this.roleField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class NewDataSet {
    
    private object[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("osm", typeof(osm))]
    [System.Xml.Serialization.XmlElementAttribute("tag", typeof(tag))]
    public object[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}
