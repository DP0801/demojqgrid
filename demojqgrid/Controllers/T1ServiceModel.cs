using System;
using System.Runtime.Serialization;

namespace demojqgrid.Controllers
{
    [DataContract]
    public class T1ServiceModel
    {
        [DataMember(Name = "Id")]
        public Int32 Id { get; set; }

        [DataMember(Name = "ProgramID")]
        public string ProgramID { get; set; }

        [DataMember(Name = "ProgramName")]
        public string ProgramName { get; set; }

        [DataMember(Name = "Key")]
        public string Key { get; set; }

        [DataMember(Name = "Value")]
        public string Value { get; set; }

        [DataMember(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}