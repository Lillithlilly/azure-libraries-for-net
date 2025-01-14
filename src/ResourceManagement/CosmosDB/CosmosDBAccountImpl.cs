// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
namespace Microsoft.Azure.Management.CosmosDB.Fluent
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Management.CosmosDB.Fluent.CosmosDBAccount.Definition;
    using Microsoft.Azure.Management.CosmosDB.Fluent.CosmosDBAccount.Update;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using System.Collections.Generic;
    using ResourceManager.Fluent.Core.Resource.Update;
    using System;
    using Microsoft.Azure.Management.CosmosDB.Fluent.Models;

    /// <summary>
    /// The implementation for DatabaseAccount.
    /// </summary>
    public partial class CosmosDBAccountImpl :
        GroupableResource<
            ICosmosDBAccount,
            Models.DatabaseAccountInner,
            CosmosDBAccountImpl,
            ICosmosDBManager,
            IWithGroup,
            IWithKind,
            IWithCreate,
            IUpdate>,
        ICosmosDBAccount,
        IDefinition,
        IUpdate
    {
        private IList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.FailoverPolicyInner> failoverPolicies;
        private bool hasFailoverPolicyChanges;
        private const int maxDelayDueToMissingFailovers = 5000 * 12 * 10;

        public CosmosDBAccountImpl WithReadReplication(Region region)
        {
            this.EnsureFailoverIsInitialized();
            Models.FailoverPolicyInner failoverPolicyInner = new Models.FailoverPolicyInner();
            failoverPolicyInner.LocationName = region.Name;
            failoverPolicyInner.FailoverPriority = this.failoverPolicies.Count;
            this.hasFailoverPolicyChanges = true;
            this.failoverPolicies.Add(failoverPolicyInner);
            return this;
        }

        private async Task<Microsoft.Azure.Management.CosmosDB.Fluent.ICosmosDBAccount> DoDatabaseUpdateCreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            CosmosDBAccountImpl self = this;
            int currentDelayDueToMissingFailovers = 0;
            Models.DatabaseAccountCreateUpdateParametersInner createUpdateParametersInner =
                this.CreateUpdateParametersInner(this.Inner);
            await this.Manager.Inner.DatabaseAccounts.CreateOrUpdateAsync(
                ResourceGroupName, Name, createUpdateParametersInner);
            this.failoverPolicies.Clear();
            this.hasFailoverPolicyChanges = false;
            bool done = false;
            ICosmosDBAccount databaseAccount = null;
            while (!done)
            {
                await SdkContext.DelayProvider.DelayAsync(5000, cancellationToken);
                databaseAccount = await this.Manager.CosmosDBAccounts.GetByResourceGroupAsync(
                    ResourceGroupName, Name);

                if (maxDelayDueToMissingFailovers > currentDelayDueToMissingFailovers &&
                    (databaseAccount.Id == null
                    || databaseAccount.Id.Length == 0
                    || createUpdateParametersInner.Locations.Count >
                        databaseAccount.Inner.FailoverPolicies.Count))
                {
                    currentDelayDueToMissingFailovers += 5000;
                    continue;
                }

                if (this.IsProvisioningStateFinal(databaseAccount.Inner.ProvisioningState))
                {
                    done = true;
                    foreach (Models.Location location in databaseAccount.ReadableReplications)
                    {
                        if (!this.IsProvisioningStateFinal(location.ProvisioningState))
                        {
                            done = false;
                            break;
                        }
                    }
                }
            }

            this.SetInner(databaseAccount.Inner);
            this.initializeFailover();
            return databaseAccount;
        }

        private Models.DatabaseAccountCreateUpdateParametersInner CreateUpdateParametersInner(Models.DatabaseAccountInner inner)
        {
            this.EnsureFailoverIsInitialized();
            Models.DatabaseAccountCreateUpdateParametersInner createUpdateParametersInner =
            new Models.DatabaseAccountCreateUpdateParametersInner();
            createUpdateParametersInner.Location = this.RegionName.ToLower();
            createUpdateParametersInner.ConsistencyPolicy = inner.ConsistencyPolicy;
            createUpdateParametersInner.IpRangeFilter = inner.IpRangeFilter;
            createUpdateParametersInner.Kind = inner.Kind;
            createUpdateParametersInner.Capabilities = inner.Capabilities;
            createUpdateParametersInner.Tags = inner.Tags;
            this.AddLocationsForCreateUpdateParameters(createUpdateParametersInner, this.failoverPolicies);
            return createUpdateParametersInner;
        }

        private bool IsProvisioningStateFinal(string state)
        {
            switch (state.ToLower())
            {
                case "succeeded":
                case "canceled":
                case "failed":
                    return true;
                default:
                    return false;
            }
        }

        public IReadOnlyList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.Location> WritableReplications()
        {
            return this.Inner.WriteLocations as IReadOnlyList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.Location>;
        }

        public async Task<Microsoft.Azure.Management.CosmosDB.Fluent.IDatabaseAccountListKeysResult> ListKeysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.Manager.Inner.DatabaseAccounts.ListKeysAsync(this.ResourceGroupName,
                this.Name);
            return result != null ? new DatabaseAccountListKeysResultImpl(result) : null;
        }

        public override async Task<Microsoft.Azure.Management.CosmosDB.Fluent.ICosmosDBAccount> CreateResourceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.DoDatabaseUpdateCreateAsync(cancellationToken);
        }

        public IDatabaseAccountListKeysResult ListKeys()
        {
            return Extensions.Synchronize(() => this.ListKeysAsync());
        }

        public string IPRangeFilter()
        {
            return this.Inner.IpRangeFilter;
        }

        public string Kind()
        {
            return this.Inner.Kind;
        }

        private void EnsureFailoverIsInitialized()
        {
            if (this.IsInCreateMode)
            {
                return;
            }

            if (!this.hasFailoverPolicyChanges)
            {
                this.initializeFailover();
            }
        }

        private void initializeFailover()
        {
            this.failoverPolicies.Clear();
            Models.FailoverPolicyInner[] policyInners = new Models.FailoverPolicyInner[this.Inner.FailoverPolicies.Count];
            for (var i = 0; i < policyInners.Length; i++)
            {
                policyInners[i] = this.Inner.FailoverPolicies[i];
            }

            Array.Sort(policyInners, (o1, o2) =>
            {
                return o1.FailoverPriority.GetValueOrDefault().CompareTo(o2.FailoverPriority.GetValueOrDefault());
            });

            for (int i = 0; i < policyInners.Length; i++)
            {
                this.failoverPolicies.Add(policyInners[i]);
            }

            this.hasFailoverPolicyChanges = true;
        }

        public CosmosDBAccountImpl WithStrongConsistency()
        {
            this.SetConsistencyPolicy(Models.DefaultConsistencyLevel.Strong, 0, 0);
            return this;
        }

        private void SetConsistencyPolicy(Models.DefaultConsistencyLevel level, int maxIntervalInSeconds, long maxStalenessPrefix)
        {
            var policy = new Models.ConsistencyPolicy();
            policy.DefaultConsistencyLevel = level;
            if (level == Models.DefaultConsistencyLevel.BoundedStaleness)
            {
                policy.MaxIntervalInSeconds = maxIntervalInSeconds;
                policy.MaxStalenessPrefix = (long)maxStalenessPrefix;
            }

            this.Inner.ConsistencyPolicy = policy;
        }

        public async Task<Microsoft.Azure.Management.CosmosDB.Fluent.IDatabaseAccountListConnectionStringsResult> ListConnectionStringsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.Manager.Inner.DatabaseAccounts.ListConnectionStringsAsync(this.ResourceGroupName,
                this.Name);
            return result != null ? new DatabaseAccountListConnectionStringsResultImpl(result) : null;
        }

        public async Task RegenerateKeyAsync(string keyKind, CancellationToken cancellationToken = default(CancellationToken))
        {
            await this.Manager.Inner.DatabaseAccounts.RegenerateKeyAsync(this.ResourceGroupName, this.Name, keyKind);
        }

        public CosmosDBAccountImpl WithKind(string kind)
        {
            this.Inner.Kind = kind;
            return this;
        }

        public CosmosDBAccountImpl WithEventualConsistency()
        {
            this.SetConsistencyPolicy(Models.DefaultConsistencyLevel.Eventual, 0, 0);
            return this;
        }

        public CosmosDBAccountImpl WithBoundedStalenessConsistency(int maxStalenessPrefix, int maxIntervalInSeconds)
        {
            this.SetConsistencyPolicy(Models.DefaultConsistencyLevel.BoundedStaleness,
                maxStalenessPrefix,
                maxIntervalInSeconds);
            return this;
        }

        public Models.DefaultConsistencyLevel DefaultConsistencyLevel()
        {
            if (this.Inner.ConsistencyPolicy == null)
            {
                throw new Exception("Consistency policy is missing!");
            }

            return this.Inner.ConsistencyPolicy.DefaultConsistencyLevel;
        }

        public IDatabaseAccountListConnectionStringsResult ListConnectionStrings()
        {
            return Extensions.Synchronize(() => this.ListConnectionStringsAsync());
        }

        internal CosmosDBAccountImpl(string name, Models.DatabaseAccountInner innerObject, ICosmosDBManager manager) :
            base(name, innerObject, manager)
        {
            this.failoverPolicies = new List<Models.FailoverPolicyInner>();
        }

        public Models.ConsistencyPolicy ConsistencyPolicy()
        {
            return this.Inner.ConsistencyPolicy;
        }

        public CosmosDBAccountImpl WithoutReadReplication(Region region)
        {
            this.EnsureFailoverIsInitialized();
            for (int i = 1; i < this.failoverPolicies.Count; i++)
            {
                if (this.failoverPolicies[i].LocationName != null)
                {
                    string locName = this.failoverPolicies[i].LocationName.Replace(" ", "").ToLower();
                    if (locName.Equals(region.Name))
                    {
                        this.failoverPolicies.RemoveAt(i);
                    }
                }
            }

            return this;
        }

        public CosmosDBAccountImpl WithSessionConsistency()
        {
            this.SetConsistencyPolicy(Models.DefaultConsistencyLevel.Session, 0, 0);
            return this;
        }

        private void AddLocationsForCreateUpdateParameters(Models.DatabaseAccountCreateUpdateParametersInner createUpdateParametersInner, IList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.FailoverPolicyInner> failoverPolicies)
        {
            List<Models.Location> locations = new List<Models.Location>();
            for (int i = 0; i < failoverPolicies.Count; i++)
            {
                Models.FailoverPolicyInner policyInner = failoverPolicies[i];
                Models.Location location = new Models.Location();
                location.FailoverPriority = i;
                location.LocationName = policyInner.LocationName;
                locations.Add(location);
            }

            if (locations.Count > 0)
            {
                createUpdateParametersInner.Locations = locations;
            }
        }

        public CosmosDBAccountImpl WithIpRangeFilter(string ipRangeFilter)
        {
            this.Inner.IpRangeFilter = ipRangeFilter;
            return this;
        }

        public CosmosDBAccountImpl WithWriteReplication(Region region)
        {
            Models.FailoverPolicyInner failoverPolicyInner = new Models.FailoverPolicyInner();
            failoverPolicyInner.LocationName = region.Name;
            this.hasFailoverPolicyChanges = true;
            this.failoverPolicies.Add(failoverPolicyInner);
            return this;
        }

        public string DocumentEndpoint()
        {
            return this.Inner.DocumentEndpoint;
        }

        public IReadOnlyList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.Location> ReadableReplications()
        {
            return this.Inner.ReadLocations as IReadOnlyList<Microsoft.Azure.Management.CosmosDB.Fluent.Models.Location>;
        }

        protected override async Task<Microsoft.Azure.Management.CosmosDB.Fluent.Models.DatabaseAccountInner> GetInnerAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.Manager.Inner.DatabaseAccounts.GetAsync(this.ResourceGroupName, this.Name);
        }

        public Models.DatabaseAccountOfferType DatabaseAccountOfferType()
        {
            return this.Inner.DatabaseAccountOfferType.GetValueOrDefault();
        }

        public void RegenerateKey(string keyKind)
        {
            Extensions.Synchronize(() => this.RegenerateKeyAsync(keyKind));
        }

        private static string FixDBName(string name)
        {
            return name.ToLower();
        }

        IWithOptionals IUpdateWithTags<IWithOptionals>.WithTags(IDictionary<string, string> tags)
        {
            this.WithTags(tags);
            return this;
        }

        IWithOptionals IUpdateWithTags<IWithOptionals>.WithTag(string key, string value)
        {
            this.WithTag(key, value);
            return this;
        }

        IWithOptionals IUpdateWithTags<IWithOptionals>.WithoutTag(string key)
        {
            this.WithoutTag(key);
            return this;
        }

        ///GENMHASH:199BC8B250DE6FDC60059ADB2A4D8A17:0D023FA55B68AD0828AD9BF823383A9A
        public IDatabaseAccountListReadOnlyKeysResult ListReadOnlyKeys()
        {
            return Extensions.Synchronize(() => this.ListReadOnlyKeysAsync());
        }

        ///GENMHASH:53B98D29180D0703E1A1842F17ACDE80:FC86BA20A774722CAD5A76DA690B6B40
        public async Task<Microsoft.Azure.Management.CosmosDB.Fluent.IDatabaseAccountListReadOnlyKeysResult> ListReadOnlyKeysAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await this.Manager.Inner.DatabaseAccounts
                .ListReadOnlyKeysAsync(this.ResourceGroupName, this.Name);
            return result != null ? new DatabaseAccountListReadOnlyKeysResultImpl(result) : null;
        }


        ///GENMHASH:6A08D79B3D058AD12B94D8E88D3A66BB:CBB08B5D2F8EBB6B1A4BE51DA2907810
        public CosmosDBAccountImpl WithDataModelGremlin()
        {
            this.Inner.Kind = DatabaseAccountKind.Others.Value;
            List<Capability> capabilities = new List<Capability>();
            capabilities.Add(new Capability("EnableGremlin"));
            this.Inner.Capabilities = capabilities;

            return this;
        }

        ///GENMHASH:34B523C69C7FD510214D27D478D971AA:49F5455D8963DDB2BE21EA8B38B4C7BE
        public CosmosDBAccountImpl WithDataModelCassandra()
        {
            this.Inner.Kind = DatabaseAccountKind.Others.Value;
            List<Capability> capabilities = new List<Capability>();
            capabilities.Add(new Capability("EnableCassandra"));
            this.Inner.Capabilities = capabilities;

            return this;
        }

        ///GENMHASH:FE98547B907685F667775CEF9663148D:443A834D31456201597F04A15B4BD4A4
        public CosmosDBAccountImpl WithDataModelMongoDB()
        {
            this.Inner.Kind = DatabaseAccountKind.MongoDB.Value;

            return this;
        }

        ///GENMHASH:D21A1256B8AE75B30461590AB84F759B:5B9CF5267A2EC5C6DB95D90298E3ADB2
        public CosmosDBAccountImpl WithDataModelSql()
        {
            this.Inner.Kind = DatabaseAccountKind.GlobalDocumentDB.Value;

            return this;
        }

        ///GENMHASH:CA81479D1B8439CD804916091691404E:3032A4A8923DA7CE6CCD1FF98076F538
        public CosmosDBAccountImpl WithDataModelAzureTable()
        {
            this.Inner.Kind = DatabaseAccountKind.Others.Value;
            List<Capability> capabilities = new List<Capability>();
            capabilities.Add(new Capability("EnableTable"));
            this.Inner.Capabilities = capabilities;

            return this;
        }

        ///GENMHASH:A86A41D6B877011AC6B43DCB0D23965B:1E42DEC842C982C7303E6EE753676F51
        public CosmosDBAccountImpl WithKind(DatabaseAccountKind kind, params Capability[] capabilities)
        {
            this.Inner.Kind = kind.Value;
            this.Inner.Capabilities = capabilities;
            return this;
        }

        ///GENMHASH:35EB1A31F5F9EE9C1A764577CD186B0D:C38E77AA90C47C0D1306953EF1EEE431
        public IReadOnlyList<Models.Capability> Capabilities()
        {
            List<Capability> capabilities = new List<Capability>(this.Inner.Capabilities);
            return capabilities.AsReadOnly();
        }

    }
}