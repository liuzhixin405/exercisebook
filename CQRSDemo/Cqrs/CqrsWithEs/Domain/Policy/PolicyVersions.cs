namespace CqrsWithEs.Domain.Policy
{
    public static class PolicyVersions
    {
        public static PolicyVersion EffectiveAt(this IEnumerable<PolicyVersion> versions,DateTime effectiveDateOfChange)=>
             versions.Where(version =>version.VersionStatus ==  PolicyVersionStatus.Active)
                .Where(version =>version.IsEffectiveOn(effectiveDateOfChange))
                .OrderByDescending(version => version.VersionNumber)
                .FirstOrDefault();
        
        public static PolicyVersion WithNumber(this IEnumerable<PolicyVersion> versions,int versionNumber)=>
            versions.FirstOrDefault(version => version.VersionNumber == versionNumber);
        public static PolicyVersion LastActive(this IEnumerable<PolicyVersion> versions)=>
            versions.Where(version=>version.VersionStatus == PolicyVersionStatus.Active).OrderByDescending(version => version.VersionNumber).FirstOrDefault();
            

    }
}
