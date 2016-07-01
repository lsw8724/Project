using System;

namespace TestCMSCommon.NHibernate
{
    public class JoinMaster // : DataEntityBase<Int32>
    {
        public virtual string Name { get; set; }

        public virtual string NickName { get; set; }

        public virtual string Description { get; set; }
    }
}
