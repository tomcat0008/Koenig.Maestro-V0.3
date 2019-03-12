using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Koenig.Maestro.Entity
{
    public abstract class DbEntityBase: ITransactionEntity
    {
        protected string TostringTemplate
        {
            get
            {
                return string.Format("Type:`{0}`, Id: {1}", this.GetType().Name, Id);
            }
        }

        [JsonConversionTarget]
        [DisplayProperty(Text = "Id", DataField = "Id",Sort = true, DisplayOrder =0, Align="left" )]
        public long Id { get; set; }
        public string CreatedUser { get; set; }
        public string UpdatedUser { get; set; }
        //[DisplayProperty(Name = "Create date", Selector = "CreateDate",Sortable = true, Center = false)]
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string RecordStatus { get; set; }
        
        public string TypeName
        {
            get
            {
                return this.GetType().Name;
            }
        }

    }
}
