using Milvasoft.Helpers.DataAccess.Concrete.Entity;
using MilvaTemplate.Entity.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilvaTemplate.Entity
{
    /// <summary>
    /// Busines Logic based information : Languages added to the system.
    /// </summary>
    [Table(TableNames.SystemLanguage)]
    public class SystemLanguage : FullAuditableEntity<MilvaTemplateUser, Guid, int>
    {
        /// <summary>
        /// Name of language. (e.g. Turkish, English)
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// Defines the ISO code of that language. (e.g. en-US)
        /// </summary>
        public string IsoCode { get; set; }
    }
}
