﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.InvoiceUpdateReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV")]
    public interface ZWS_SPU_INVOICE_POST_SRV {
        
        // CODEGEN: Generating message contract since the operation ZFM_SPU_INVOICE_POST is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_SPU_INVOICE_POST_SRV:ZFM_SPU_INVOICE_P" +
            "OSTRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1 ZFM_SPU_INVOICE_POST(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_SPU_INVOICE_POST_SRV:ZFM_SPU_INVOICE_P" +
            "OSTRequest", ReplyAction="*")]
        System.Threading.Tasks.Task<WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1> ZFM_SPU_INVOICE_POSTAsync(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZFM_SPU_INVOICE_POST : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ZSPU_INVOICE_UPD_STR[] iT_INVOICEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZSPU_INVOICE_UPD_STR[] IT_INVOICE {
            get {
                return this.iT_INVOICEField;
            }
            set {
                this.iT_INVOICEField = value;
                this.RaisePropertyChanged("IT_INVOICE");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZSPU_INVOICE_UPD_STR : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string dOCUMENTField;
        
        private string iNVOICEField;
        
        private string fyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string DOCUMENT {
            get {
                return this.dOCUMENTField;
            }
            set {
                this.dOCUMENTField = value;
                this.RaisePropertyChanged("DOCUMENT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string INVOICE {
            get {
                return this.iNVOICEField;
            }
            set {
                this.iNVOICEField = value;
                this.RaisePropertyChanged("INVOICE");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string FY {
            get {
                return this.fyField;
            }
            set {
                this.fyField = value;
                this.RaisePropertyChanged("FY");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZSPU_INVOICE_RES_STR : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string dOCUMENTField;
        
        private string mESSAGEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string DOCUMENT {
            get {
                return this.dOCUMENTField;
            }
            set {
                this.dOCUMENTField = value;
                this.RaisePropertyChanged("DOCUMENT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string MESSAGE {
            get {
                return this.mESSAGEField;
            }
            set {
                this.mESSAGEField = value;
                this.RaisePropertyChanged("MESSAGE");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZFM_SPU_INVOICE_POSTResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ZSPU_INVOICE_RES_STR[] eT_RESPONSEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZSPU_INVOICE_RES_STR[] ET_RESPONSE {
            get {
                return this.eT_RESPONSEField;
            }
            set {
                this.eT_RESPONSEField = value;
                this.RaisePropertyChanged("ET_RESPONSE");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZFM_SPU_INVOICE_POSTRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POST ZFM_SPU_INVOICE_POST;
        
        public ZFM_SPU_INVOICE_POSTRequest() {
        }
        
        public ZFM_SPU_INVOICE_POSTRequest(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POST ZFM_SPU_INVOICE_POST) {
            this.ZFM_SPU_INVOICE_POST = ZFM_SPU_INVOICE_POST;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZFM_SPU_INVOICE_POSTResponse1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse ZFM_SPU_INVOICE_POSTResponse;
        
        public ZFM_SPU_INVOICE_POSTResponse1() {
        }
        
        public ZFM_SPU_INVOICE_POSTResponse1(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse ZFM_SPU_INVOICE_POSTResponse) {
            this.ZFM_SPU_INVOICE_POSTResponse = ZFM_SPU_INVOICE_POSTResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ZWS_SPU_INVOICE_POST_SRVChannel : WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ZWS_SPU_INVOICE_POST_SRVClient : System.ServiceModel.ClientBase<WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV>, WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV {
        
        public ZWS_SPU_INVOICE_POST_SRVClient() {
        }
        
        public ZWS_SPU_INVOICE_POST_SRVClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ZWS_SPU_INVOICE_POST_SRVClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ZWS_SPU_INVOICE_POST_SRVClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ZWS_SPU_INVOICE_POST_SRVClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1 WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV.ZFM_SPU_INVOICE_POST(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest request) {
            return base.Channel.ZFM_SPU_INVOICE_POST(request);
        }
        
        public WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse ZFM_SPU_INVOICE_POST(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POST ZFM_SPU_INVOICE_POST1) {
            WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest inValue = new WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest();
            inValue.ZFM_SPU_INVOICE_POST = ZFM_SPU_INVOICE_POST1;
            WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1 retVal = ((WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV)(this)).ZFM_SPU_INVOICE_POST(inValue);
            return retVal.ZFM_SPU_INVOICE_POSTResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1> WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV.ZFM_SPU_INVOICE_POSTAsync(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest request) {
            return base.Channel.ZFM_SPU_INVOICE_POSTAsync(request);
        }
        
        public System.Threading.Tasks.Task<WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTResponse1> ZFM_SPU_INVOICE_POSTAsync(WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POST ZFM_SPU_INVOICE_POST) {
            WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest inValue = new WebApplication1.InvoiceUpdateReference.ZFM_SPU_INVOICE_POSTRequest();
            inValue.ZFM_SPU_INVOICE_POST = ZFM_SPU_INVOICE_POST;
            return ((WebApplication1.InvoiceUpdateReference.ZWS_SPU_INVOICE_POST_SRV)(this)).ZFM_SPU_INVOICE_POSTAsync(inValue);
        }
    }
}
