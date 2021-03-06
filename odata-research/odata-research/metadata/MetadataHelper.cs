﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
namespace metadata
{
    public class MetadataBuilder
    {
        private readonly EdmModel model = new EdmModel();
        public MetadataBuilder()
        {

        }
        public  MetadataBuilder   BuildAddress()

        {
            
            var customer = new EdmEntityType("metadata", "Customer");
            bool isNullable = false;
            var idProperty = customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable));
            customer.AddKeys(idProperty);
            customer.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable));
            

            customer.AddStructuralProperty("City", EdmCoreModel.Instance.GetString(isNullable));
            model.AddElement(customer);

            var container = new EdmEntityContainer("metadata", "DefaultContainer");
            container.AddEntitySet("Customers", customer);
            model.AddElement(container);
            return this;
        }
        public IEdmModel GetModel()
        {
            return model;
        }
    }
    public class Customer
    {
        public string Name
        {
            get;
            set;
        }
        public int Id
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }
    }
}
