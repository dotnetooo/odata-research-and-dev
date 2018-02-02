using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
namespace metadata
{
    public class MetadataBuilder
    {
        private readonly EdmModel _model = new EdmModel();
        public MetadataBuilder()
        {

        }
        public  MetadataBuilder   BuildAddress()
        {
            var _addressType = new EdmEntityType("metadata", "Address");
            _addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            _addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            _addressType.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.Int32);
            _model.AddElement(_addressType);
            return this;
        }
        public IEdmModel GetModel()
        {
            return _model;
        }
    }
    public class Address
    {
        public string PostalCode
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }
        public string Street
        {
            get;
            set;
        }
    }
}
