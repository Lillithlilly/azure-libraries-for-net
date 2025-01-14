﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Batch.Fluent;
using Microsoft.Azure.Management.Cdn.Fluent;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Dns.Fluent;
using Microsoft.Azure.Management.KeyVault.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Redis.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Search.Fluent;
using Microsoft.Azure.Management.ServiceBus.Fluent;
using Microsoft.Azure.Management.Sql.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Azure.Management.TrafficManager.Fluent;
using System;
using System.Linq;
using ISubscriptions = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscriptions;
using ISubscription = Microsoft.Azure.Management.ResourceManager.Fluent.ISubscription;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerRegistry.Fluent;
using Microsoft.Azure.Management.ContainerService.Fluent;
using Microsoft.Azure.Management.CosmosDB.Fluent;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.Locks.Fluent;
using Microsoft.Azure.Management.Msi.Fluent;
using Microsoft.Azure.Management.BatchAI.Fluent;
using Microsoft.Azure.Management.Monitor.Fluent;
using Microsoft.Azure.Management.Eventhub.Fluent;
using Microsoft.Azure.Management.EventHub.Fluent;

namespace Microsoft.Azure.Management.Fluent
{
    public class Azure : IAzure
    {
        private IAuthenticated authenticated;

        #region Service Managers

        private IResourceManager resourceManager;
        private IStorageManager storageManager;
        private IComputeManager computeManager;
        private INetworkManager networkManager;
        private IBatchManager batchManager;
        private IKeyVaultManager keyVaultManager;
        private ITrafficManager trafficManager;
        private IDnsZoneManager dnsZoneManager;
        private ISqlManager sqlManager;
        private ICdnManager cdnManager;
        private IRedisManager redisManager;
        private IAppServiceManager appServiceManager;
        private ISearchManager searchManager;
        private IServiceBusManager serviceBusManager;
        private IContainerInstanceManager containerInstanceManager;
        private IRegistryManager registryManager;
        private IContainerServiceManager containerServiceManager;
        private ICosmosDBManager cosmosDBManager;
        private IAuthorizationManager authorizationManager;
        private IMsiManager msiManager;
        private IBatchAIManager batchAIManager;
        private IMonitorManager monitorManager;
        private IEventHubManager eventHubManager;

        #endregion Service Managers

        #region Getters

        /// <returns>the currently selected subscription ID this client is authenticated to work with</returns>
        public string SubscriptionId
        {
            get; private set;
        }

        /// <returns>the currently selected subscription this client is authenticated to work with</returns>
        public ISubscription GetCurrentSubscription()
        {
            return Subscriptions.GetById(SubscriptionId);
        }

        /// <returns>entry point to managing locks</returns>
        public IManagementLocks ManagementLocks
        {
            get
            {
                return authorizationManager.ManagementLocks;
            }
        }

        /// <returns>entry point to managing resource groups</returns>
        public IResourceGroups ResourceGroups
        {
            get
            {
                return resourceManager.ResourceGroups;
            }
        }

        /// <returns>entry point to managing storage accounts</returns>
        public IStorageAccounts StorageAccounts
        {
            get
            {
                return storageManager.StorageAccounts;
            }
        }

        /// <returns>entry point to managing virtual machines</returns>
        public IVirtualMachines VirtualMachines
        {
            get
            {
                return computeManager.VirtualMachines;
            }
        }

        /// <returns>entry point to managing virtual machine scale sets</returns>
        public IVirtualMachineScaleSets VirtualMachineScaleSets
        {
            get
            {
                return computeManager.VirtualMachineScaleSets;
            }
        }

        /// <returns>entry point to managing virtual networks</returns>
        public INetworks Networks
        {
            get
            {
                return networkManager.Networks;
            }
        }

        /// <returns>entry point to managing network security groups</returns>
        public INetworkSecurityGroups NetworkSecurityGroups
        {
            get
            {
                return networkManager.NetworkSecurityGroups;
            }
        }

        /// <returns>entry point to managing public IP addresses</returns>
        public IPublicIPAddresses PublicIPAddresses
        {
            get
            {
                return networkManager.PublicIPAddresses;
            }
        }

        /// <returns>entry point to managing network interfaces</returns>
        public INetworkInterfaces NetworkInterfaces
        {
            get
            {
                return networkManager.NetworkInterfaces;
            }
        }

        /// <returns>entry point to managing virtual load balancers</returns>
        public ILoadBalancers LoadBalancers
        {
            get
            {
                return networkManager.LoadBalancers;
            }
        }

        /// <returns>entry point to managing application gateways</returns>
        public IApplicationGateways ApplicationGateways
        {
            get
            {
                return networkManager.ApplicationGateways;
            }
        }

        /// <returns>entry point to managing network watchers</returns>
        public INetworkWatchers NetworkWatchers
        {
            get
            {
                return networkManager.NetworkWatchers;
            }
        }

        /// <returns>entry point to managing Azure Virtual Network Gateways</returns>
        public IVirtualNetworkGateways VirtualNetworkGateways
        {
            get
            {
                return networkManager.VirtualNetworkGateways;
            }
        }

        /// <returns>entry point to managing Azure Local Network Gateways</returns>
        public ILocalNetworkGateways LocalNetworkGateways
        {
            get
            {
                return networkManager.LocalNetworkGateways;
            }
        }

        /// <returns>entry point to managing Azure Express Route Circuits</returns>
        public IExpressRouteCircuits ExpressRouteCircuits
        {
            get
            {
                return networkManager.ExpressRouteCircuits;
            }
        }

        /// <returns>entry point to managing Application Security Gropus</returns>
        public IApplicationSecurityGroups ApplicationSecurityGroups
        {
            get
            {
                return networkManager.ApplicationSecurityGroups;
            }
        }

        /// <returns>entry point to managing Route Filters</returns>
        public IRouteFilters RouteFilters
        {
            get
            {
                return networkManager.RouteFilters;
            }
        }

        /// <returns>entry point to managing DDoS protection plans</returns>
        public IDdosProtectionPlans DdosProtectionPlans
        {
            get
            {
                return networkManager.DdosProtectionPlans;
            }
        }

        /// <returns>entry point to managing deployments</returns>
        public IDeployments Deployments
        {
            get
            {
                return resourceManager.Deployments;
            }
        }

        /// <returns>entry point to managing virtual machine images</returns>
        public IVirtualMachineImages VirtualMachineImages
        {
            get
            {
                return computeManager.VirtualMachineImages;
            }
        }

        /// <returns>entry point to managing virtual machine extension images</returns>
        public IVirtualMachineExtensionImages VirtualMachineExtensionImages
        {
            get
            {
                return computeManager.VirtualMachineExtensionImages;
            }
        }

        /// <returns>entry point to managing availability sets</returns>
        public IAvailabilitySets AvailabilitySets
        {
            get
            {
                return computeManager.AvailabilitySets;
            }
        }

        /// <returns>entry point to managing Azure Batch accounts</returns>
        public IBatchAccounts BatchAccounts
        {
            get
            {
                return batchManager.BatchAccounts;
            }
        }

        /// <returns>entry point to managing Azure key vaults</returns>
        public IVaults Vaults
        {
            get
            {
                return keyVaultManager.Vaults;
            }
        }

        /// <returns>entry point to managing Traffic Manager profiles</returns>
        public ITrafficManagerProfiles TrafficManagerProfiles
        {
            get
            {
                return trafficManager.Profiles;
            }
        }

        /// <returns>entry point to managing DNS zones</returns>
        public IDnsZones DnsZones
        {
            get
            {
                return dnsZoneManager.Zones;
            }
        }

        /// <returns>entry point to managing SQL servers</returns>
        public ISqlServers SqlServers
        {
            get
            {
                return sqlManager.SqlServers;
            }
        }

        /// <returns>entry point to managing Redis caches</returns>
        public IRedisCaches RedisCaches
        {
            get
            {
                return redisManager.RedisCaches;
            }
        }

        /// <returns>entry point to managing CDN profiles</returns>
        public ICdnProfiles CdnProfiles
        {
            get
            {
                return cdnManager.Profiles;
            }
        }

        /// <returns>entry point to managing web apps</returns>
        public IWebApps WebApps
        {
            get
            {
                return appServiceManager.WebApps;
            }
        }

        /// <returns>entry point to managing app services</returns>
        public IAppServiceManager AppServices
        {
            get
            {
                return appServiceManager;
            }
        }

        /// <returns>subscriptions that this authenticated client has access to</returns>
        public ISubscriptions Subscriptions
        {
            get
            {
                return authenticated.Subscriptions;
            }
        }

        public IVirtualMachineCustomImages VirtualMachineCustomImages
        {
            get
            {
                return computeManager.VirtualMachineCustomImages;
            }
        }

        public IDisks Disks
        {
            get
            {
                return computeManager.Disks;
            }
        }

        public ISnapshots Snapshots
        {
            get
            {
                return computeManager.Snapshots;
            }
        }

        public ISearchServices SearchServices
        {
            get
            {
                return searchManager.SearchServices;
            }
        }

        public IServiceBusNamespaces ServiceBusNamespaces
        {
            get
            {
                return serviceBusManager.Namespaces;
            }
        }

        public ContainerService.Fluent.IContainerServices ContainerServices
        {
            get
            {
                return containerServiceManager.ContainerServices;
            }
        }

        public ContainerService.Fluent.IKubernetesClusters KubernetesClusters
        {
            get
            {
                return containerServiceManager.KubernetesClusters;
            }
        }

        public ICosmosDBAccounts CosmosDBAccounts
        {
            get
            {
                return cosmosDBManager.CosmosDBAccounts;
            }
        }

        public IContainerGroups ContainerGroups
        {
            get
            {
                return containerInstanceManager.ContainerGroups;
            }
        }

        public IRegistries ContainerRegistries
        {
            get
            {
                return registryManager.ContainerRegistries;
            }
        }

        public IAccessManagement AccessManagement
        {
            get
            {
                return authenticated;
            }
        }

        public IIdentities Identities
        {
            get
            {
                return msiManager.Identities;
            }
        }

        public IComputeSkus ComputeSkus
        {
            get
            {
                return computeManager.ComputeSkus;
            }
        }

        public IBatchAIClusters BatchAIClusters
        {
            get
            {
                return batchAIManager.BatchAIClusters;
            }
        }

        public IBatchAIFileServers BatchAIFileServers
        {
            get
            {
                return batchAIManager.BatchAIFileServers;
            }
        }

        public IActivityLogs ActivityLogs
        {
            get
            {
                return monitorManager.ActivityLogs;
            }
        }

        public IMetricDefinitions MetricDefinitions
        {
            get
            {
                return monitorManager.MetricDefinitions;
            }
        }

        public IDiagnosticSettings DiagnosticSettings
        {
            get
            {
                return monitorManager.DiagnosticSettings;
            }
        }

        public IActionGroups ActionGroups
        {
            get
            {
                return monitorManager.ActionGroups;
            }
        }

        public IEventHubNamespaces EventHubNamespaces
        {
            get
            {
                return eventHubManager.Namespaces;
            }
        }

        public IEventHubs EventHubs
        {
            get
            {
                return eventHubManager.EventHubs;
            }
        }

        public IEventHubDisasterRecoveryPairings EventHubDisasterRecoveryPairings
        {
            get
            {
                return eventHubManager.EventHubDisasterRecoveryPairings;
            }
        }

        #endregion Getters

        #region ctrs

        private Azure(RestClient restClient, string subscriptionId, string tenantId, IAuthenticated authenticated)
        {
            resourceManager = ResourceManager.Fluent.ResourceManager.Authenticate(restClient).WithSubscription(subscriptionId);
            storageManager = StorageManager.Authenticate(restClient, subscriptionId);
            computeManager = ComputeManager.Authenticate(restClient, subscriptionId);
            networkManager = NetworkManager.Authenticate(restClient, subscriptionId);
            batchManager = BatchManager.Authenticate(restClient, subscriptionId);
            keyVaultManager = KeyVaultManager.Authenticate(restClient, subscriptionId, tenantId);
            trafficManager = TrafficManager.Fluent.TrafficManager.Authenticate(restClient, subscriptionId);
            dnsZoneManager = DnsZoneManager.Authenticate(restClient, subscriptionId);
            sqlManager = SqlManager.Authenticate(restClient, subscriptionId);
            redisManager = RedisManager.Authenticate(restClient, subscriptionId);
            cdnManager = CdnManager.Authenticate(restClient, subscriptionId);
            appServiceManager = AppServiceManager.Authenticate(restClient, subscriptionId, tenantId);
            searchManager = SearchManager.Authenticate(restClient, subscriptionId);
            serviceBusManager = ServiceBusManager.Authenticate(restClient, subscriptionId);
            containerInstanceManager = ContainerInstanceManager.Authenticate(restClient, subscriptionId);
            registryManager = RegistryManager.Authenticate(restClient, subscriptionId);
            containerServiceManager = ContainerServiceManager.Authenticate(restClient, subscriptionId);
            cosmosDBManager = CosmosDBManager.Authenticate(restClient, subscriptionId);
            authorizationManager = AuthorizationManager.Authenticate(restClient, subscriptionId);
            msiManager = MsiManager.Authenticate(restClient, subscriptionId);
            batchAIManager = BatchAIManager.Authenticate(restClient, subscriptionId);
            monitorManager = MonitorManager.Authenticate(restClient, subscriptionId);
            eventHubManager = EventHubManager.Authenticate(restClient, subscriptionId);

            SubscriptionId = subscriptionId;
            this.authenticated = authenticated;
        }

        #endregion ctrs

        #region Azure builder

        private static Authenticated CreateAuthenticated(RestClient restClient, string tenantId)
        {
            return new Authenticated(restClient, tenantId);
        }

        public static IAuthenticated Authenticate(AzureCredentials azureCredentials)
        {
            var authenticated = CreateAuthenticated(RestClient.Configure()
                    .WithEnvironment(azureCredentials.Environment)
                    .WithCredentials(azureCredentials)
                    .Build(), azureCredentials.TenantId);
            authenticated.SetDefaultSubscription(azureCredentials.DefaultSubscriptionId);
            return authenticated;
        }

        public static IAuthenticated Authenticate(string authFile)
        {
            AzureCredentials credentials = SdkContext.AzureCredentialsFactory.FromFile(authFile);
            return Authenticate(credentials);
        }

        public static IAuthenticated Authenticate(RestClient restClient, string tenantId)
        {
            return CreateAuthenticated(restClient, tenantId);
        }

        public static IConfigurable Configure()
        {
            return new Configurable();
        }

        #endregion Azure builder

        #region IAuthenticated and it's implementation

        public interface IAuthenticated : IAccessManagement
        {
            string TenantId { get; }

            ITenants Tenants { get; }

            ISubscriptions Subscriptions { get; }

            IAzure WithSubscription(string subscriptionId);

            IAzure WithDefaultSubscription();
        }

        protected class Authenticated : IAuthenticated
        {
            private RestClient restClient;
            private ResourceManager.Fluent.ResourceManager.IAuthenticated resourceManagerAuthenticated;
            private IGraphRbacManager graphRbacManager;
            private string defaultSubscription;
            private string tenantId;

            public string TenantId
            {
                get
                {
                    return tenantId;
                }
            }

            public ITenants Tenants
            {
                get
                {
                    return resourceManagerAuthenticated.Tenants;
                }
            }

            public ISubscriptions Subscriptions
            {
                get
                {
                    return resourceManagerAuthenticated.Subscriptions;
                }
            }

            public IActiveDirectoryUsers ActiveDirectoryUsers
            {
                get
                {
                    return graphRbacManager.Users;
                }
            }

            public IActiveDirectoryGroups ActiveDirectoryGroups
            {
                get
                {
                    return graphRbacManager.Groups;
                }
            }

            public IActiveDirectoryApplications ActiveDirectoryApplications
            {
                get
                {
                    return graphRbacManager.Applications;
                }
            }

            public IServicePrincipals ServicePrincipals
            {
                get
                {
                    return graphRbacManager.ServicePrincipals;
                }
            }

            public IRoleDefinitions RoleDefinitions
            {
                get
                {
                    return graphRbacManager.RoleDefinitions;
                }
            }

            public IRoleAssignments RoleAssignments
            {
                get
                {
                    return graphRbacManager.RoleAssignments;
                }
            }

            public Authenticated(RestClient restClient, string tenantId)
            {
                this.restClient = restClient;
                resourceManagerAuthenticated = ResourceManager.Fluent.ResourceManager.Authenticate(this.restClient);
                graphRbacManager = GraphRbacManager.Authenticate(this.restClient, tenantId);
                this.tenantId = tenantId;
            }

            public void SetDefaultSubscription(string subscriptionId)
            {
                defaultSubscription = subscriptionId;
            }

            public IAzure WithSubscription(string subscriptionId)
            {
                return new Azure(restClient, subscriptionId, tenantId, this);
            }

            public IAzure WithDefaultSubscription()
            {
                if (!string.IsNullOrWhiteSpace(defaultSubscription))
                {
                    return WithSubscription(defaultSubscription);
                }
                else
                {
                    var resourceManager = ResourceManager.Fluent.ResourceManager.Authenticate(
                        RestClient.Configure()
                            .WithBaseUri(restClient.BaseUri)
                            .WithCredentials(restClient.Credentials).Build());
                    var subscription = resourceManager.Subscriptions.List()
                        .FirstOrDefault(s =>
                            StringComparer.OrdinalIgnoreCase.Equals(s.State, "Enabled") ||
                            StringComparer.OrdinalIgnoreCase.Equals(s.State, "Warned"));

                    return WithSubscription(subscription?.SubscriptionId);
                }
            }
        }

        #endregion IAuthenticated and it's implementation

        #region IConfigurable and it's implementation

        public interface IConfigurable : IAzureConfigurable<IConfigurable>
        {
            IAuthenticated Authenticate(AzureCredentials azureCredentials);
        }

        protected class Configurable :
            AzureConfigurable<IConfigurable>,
            IConfigurable
        {
            IAuthenticated IConfigurable.Authenticate(AzureCredentials credentials)
            {
                var authenticated = new Authenticated(BuildRestClient(credentials), credentials.TenantId);
                authenticated.SetDefaultSubscription(credentials.DefaultSubscriptionId);
                return authenticated;
            }
        }

        #endregion IConfigurable and it's implementation
    }

    /// <summary>
    /// Members of IAzure that are in Beta
    /// </summary>
    public interface IAzureBeta : IBeta
    {
        /// <summary>
        /// Entry point to Azure Network Watcher management
        /// </summary>
        INetworkWatchers NetworkWatchers { get; }

        /// <summary>
        /// Entry point to Azure Virtual Network Gateways management
        /// </summary>
        IVirtualNetworkGateways VirtualNetworkGateways { get; }

        /// <summary>
        /// Entry point to Azure Local Network Gateways management
        /// </summary>
        ILocalNetworkGateways LocalNetworkGateways { get; }

        /// <summary>
        /// Entry point to Azure Express Route Circuits management
        /// </summary>
        IExpressRouteCircuits ExpressRouteCircuits { get; }

        /// <summary>
        /// Entry point to Application Security Groups management
        /// </summary>
        IApplicationSecurityGroups ApplicationSecurityGroups{ get; }

        /// <summary>
        /// Entry point to Route Filters management
        /// </summary>
        IRouteFilters RouteFilters { get; }

        /// <summary>
        /// Entry point to Azure DDoS protection plans management
        /// </summary>
        IDdosProtectionPlans DdosProtectionPlans { get; }

        /// <summary>
        /// Entry point to Azure Web App management.
        /// </summary>
        IWebApps WebApps { get; }

        /// <summary>
        /// Entry point to Azure App Service management.
        /// </summary>
        IAppServiceManager AppServices { get; }

        /// <summary>
        /// Entry point to Azure Search management.
        /// </summary>
        ISearchServices SearchServices { get; }

        /// <summary>
        /// Entry point to Azure Container Services management.
        /// </summary>
        ContainerService.Fluent.IContainerServices ContainerServices { get; }

        /// <summary>
        /// Entry point to Azure Container Services (AKS) management.
        /// </summary>
        ContainerService.Fluent.IKubernetesClusters KubernetesClusters { get; }

        /// <summary>
        /// Entry point to CosmosDB account management
        /// </summary>
        ICosmosDBAccounts CosmosDBAccounts { get; }

        /// <summary>
        /// Entry point to Azure container instance management.
        /// </summary>
        IContainerGroups ContainerGroups { get; }

        /// <summary>
        /// Entry point to Azure container registry management.
        /// </summary>
        IRegistries ContainerRegistries { get; }

        /// <summary>
        /// Entry point to managing locks.
        /// </summary>
        IManagementLocks ManagementLocks { get; }

        /// <summary>
        /// Entry point to Managed Service Identity (MSI) management.
        /// </summary>
        IIdentities Identities { get; }

        /// <summary>
        /// Entry point to Batch AI clusters management.
        /// </summary>
        IBatchAIClusters BatchAIClusters { get; }

        /// <summary>
        /// Entry point to Batch AI file servers management.
        /// </summary>
        IBatchAIFileServers BatchAIFileServers { get; }

        /// <summary>
        /// Entry point to listing activity log events in Azure.
        /// </summary>
        IActivityLogs ActivityLogs { get; }

        /// <summary>
        /// Entry point to listing metric definitions in Azure
        /// </summary>
        IMetricDefinitions MetricDefinitions { get; }

        /// <summary>
        /// Entry point to Azure Diagnostic Settings management.
        /// </summary>
        IDiagnosticSettings DiagnosticSettings { get; }

        /// <summary>
        /// Entry point to Azure Action Groups management.
        /// </summary>
        IActionGroups ActionGroups { get; }
    }

    public interface IAzure : IAzureBeta
    {
        string SubscriptionId { get; }

        /// <summary>
        /// Entry point to Azure Service Bus namespace management.
        /// </summary>
        IServiceBusNamespaces ServiceBusNamespaces { get; }

        /// <summary>
        /// Entry point to load balancer management.
        /// </summary>
        ILoadBalancers LoadBalancers { get; }

        /// <summary>
        /// Entry point to authentication and authorization management in Azure
        /// </summary>
        IAccessManagement AccessManagement { get; }

        /// <summary>
        /// Entry point to application gateway management
        /// </summary>
        IApplicationGateways ApplicationGateways { get; }

        /// <summary>
        /// Returns the subscription the API is currently configured to work with.
        /// </summary>
        /// <returns></returns>
        ISubscription GetCurrentSubscription();

        ISubscriptions Subscriptions { get; }

        /// <summary>
        /// Entry point to resource group management.
        /// </summary>
        IResourceGroups ResourceGroups { get; }

        /// <summary>
        /// Entry point to storage account management.
        /// </summary>
        IStorageAccounts StorageAccounts { get; }

        /// <summary>
        /// Entry point to virtual machine management.
        /// </summary>
        IVirtualMachines VirtualMachines { get; }

        /// <summary>
        /// Entry point to virtual machine scale set management.
        /// </summary>
        IVirtualMachineScaleSets VirtualMachineScaleSets { get; }

        /// <summary>
        /// Entry point to virtual network management.
        /// </summary>
        INetworks Networks { get; }

        /// <summary>
        /// Entry point to network security group management.
        /// </summary>
        INetworkSecurityGroups NetworkSecurityGroups { get; }

        /// <summary>
        /// Entry point to public IP address management.
        /// </summary>
        IPublicIPAddresses PublicIPAddresses { get; }

        /// <summary>
        /// Entry point to network interface management
        /// </summary>
        INetworkInterfaces NetworkInterfaces { get; }

        /// <summary>
        /// Entry point to Azure Resource Manager template deployment management.
        /// </summary>
        IDeployments Deployments { get; }

        /// <summary>
        /// Entry point to virtual machine image management.
        /// </summary>
        IVirtualMachineImages VirtualMachineImages { get; }

        /// <summary>
        /// Entry point to virtual machine extendion image management.
        /// </summary>
        IVirtualMachineExtensionImages VirtualMachineExtensionImages { get; }

        /// <summary>
        /// Entry point to availability set management.
        /// </summary>
        IAvailabilitySets AvailabilitySets { get; }

        /// <summary>
        /// Entry point to batch account management.
        /// </summary>
        IBatchAccounts BatchAccounts { get; }

        /// <summary>
        /// Entry point to Azure Key Vault management.
        /// </summary>
        IVaults Vaults { get; }

        /// <summary>
        /// Entry point to Azure Traffic Manager management.
        /// </summary>
        ITrafficManagerProfiles TrafficManagerProfiles { get; }

        /// <summary>
        /// Entry point to DNS zone management.
        /// </summary>
        IDnsZones DnsZones { get; }

        /// <summary>
        /// Entry point to Azure SQL server management.
        /// </summary>
        ISqlServers SqlServers { get; }

        /// <summary>
        /// Entry point to Azure Redis cache management.
        /// </summary>
        IRedisCaches RedisCaches { get; }

        /// <summary>
        /// Entry poing to content delivery network management.
        /// </summary>
        ICdnProfiles CdnProfiles { get; }

        /// <summary>
        /// Entry point to virtual machine custom image management.
        /// </summary>
        IVirtualMachineCustomImages VirtualMachineCustomImages { get; }

        /// <summary>
        /// Entry point to virtual machine managed disk management.
        /// </summary>
        IDisks Disks { get; }

        /// <summary>
        /// Entry point to virtual machine managed disk snapshot management.
        /// </summary>
        ISnapshots Snapshots { get; }

        /// <summary>
        /// Entry point to compute service SKU management.
        /// </summary>
        IComputeSkus ComputeSkus { get; }

        /// <summary>
        /// Entry point to Event Hub namespace management.
        /// </summary>
        IEventHubNamespaces EventHubNamespaces { get; }

        /// <summary>
        /// Entry point to Event Hub management.
        /// </summary>
        IEventHubs EventHubs { get; }

        /// <summary>
        /// Entry point to Event Hub disaster recovery pairing.
        /// </summary>
        IEventHubDisasterRecoveryPairings EventHubDisasterRecoveryPairings { get; }
    }
}
