using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xrm.Framework.CI.Common.Logging;
using Xrm.Typed.Entities;
using static Xrm.Typed.Entities.SolutionComponent;

namespace Xrm.Framework.CI.Extensions.DataOperations
{
    public class SolutionManager
    {
        #region Member Variables and Constructors
        private IOrganizationService _crmService;
        private ILogger _logger;

        public SolutionManager(IOrganizationService crmService, ILogger logger)
        {
            _crmService = crmService;
            _logger = logger;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This methods will generate the holding solutions for a CRM Instance
        /// Note: This should not ordinarly be needed.  This tool ezists for legacy deployments
        /// where for whatever reason; all components cannot be included in a single solution
        /// </summary>
        public void GenerateHoldingSolutions()
        {

        }
        #endregion

        #region Temp
        IOrganizationService _organizationService;
        public SolutionManager(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        Solution _holding_webresources = null;
        Solution _holding_assemblies = null;
        Solution _holding_optionsets = null;

        List<SolutionComponent> _holdingSolutionComponents;
        List<SolutionComponent> _areaSolutionsComponents;

        public List<SolutionComponent> HoldingSolutionComponents { get => _holdingSolutionComponents; set => _holdingSolutionComponents = value; }
        public List<SolutionComponent> AreaSolutionComponents { get => _areaSolutionsComponents; set => _areaSolutionsComponents = value; }

        //Retrieve the contents of the Holding and Area solutions
        //Create the holding solutions if they do not exist
        private void InitializeHoldingSolutions()
        {
            //Get references to the top level holding solutions
            _holding_webresources = GetOrCreateSolution("holding_webresources", "DefaultPublisher", "Holding Solution: Web Resources");
            _holding_assemblies = GetOrCreateSolution("holding_assemblies", "DefaultPublisher", "Holding Solution: Assemblies");
            _holding_optionsets = GetOrCreateSolution("holding_optionsets", "DefaultPublisher", "Holding Solution: Optionsets");

            using (OrganizationServiceContext xrmContext = new OrganizationServiceContext(_organizationService))
            {
                //Retrieve all the components already in a solution
                _holdingSolutionComponents = (
                    from sc in xrmContext.CreateQuery<SolutionComponent>()
                    join s in xrmContext.CreateQuery<Solution>()
                        on sc.SolutionId.Id equals s.SolutionId
                    where s.UniqueName.StartsWith("holding_")
                    select sc).ToList();

                //Retrieve all area solutions
                _areaSolutionsComponents = (
                    from sc in xrmContext.CreateQuery<SolutionComponent>()
                    join s in xrmContext.CreateQuery<Solution>()
                        on sc.SolutionId.Id equals s.SolutionId
                    where s.UniqueName.StartsWith("area_")
                    select sc).ToList();
            }
        }

        //Update holding solutions with new content
        public void UpdateHoldingSolutions()
        {
            InitializeHoldingSolutions();
            using (OrganizationServiceContext xrmContext = new OrganizationServiceContext(_organizationService))
            {
                //Retrieve components for holding solutions
                var pluginTypes = (
                    from pt in xrmContext.CreateQuery<PluginType>()
                    where pt.IsManaged == false
                    orderby pt.Name
                    select new PluginType() { PluginTypeId = pt.PluginTypeId, Name = pt.Name }).ToList();

                var pluginAssemblies = (
                    from pa in xrmContext.CreateQuery<PluginAssembly>()
                    where pa.IsManaged == false
                    orderby pa.Name
                    select new PluginAssembly()
                    {
                        PluginAssemblyId = pa.PluginAssemblyId
                        ,
                        Name = pa.Name
                    }).ToList();

                var webresources = (
                    from wr in xrmContext.CreateQuery<WebResource>()
                    where wr.IsManaged == false
                    orderby wr.Name
                    select new WebResource() { WebResourceId = wr.WebResourceId, Name = wr.Name }).ToList();

                // Retrieve Optionset Values
                RetrieveAllOptionSetsRequest retrieveAllOptionSetsRequest = new RetrieveAllOptionSetsRequest();
                RetrieveAllOptionSetsResponse retrieveAllOptionSetsResponse = (RetrieveAllOptionSetsResponse)_organizationService.Execute(retrieveAllOptionSetsRequest);

                //Add webresources to holding solutions
                foreach (var webresource in webresources)
                {
                    var solutionWebresource = HoldingSolutionComponents.Where(sc => sc.ObjectId == webresource.Id).FirstOrDefault();
                    if (solutionWebresource == null)
                    {
                        _logger.LogInformation($"Solution: {_holding_webresources.UniqueName}. Including webresource '{webresource.Name}'");
                        AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
                        {
                            ComponentType = (int)componenttypeValues.WebResource,
                            ComponentId = webresource.Id,
                            SolutionUniqueName = _holding_webresources.UniqueName
                        };
                        _organizationService.Execute(addReq);
                    }
                    else
                    {
                        _logger.LogInformation($"Solution: {_holding_webresources.UniqueName}. Webresource already exists: '{webresource.Name}'");
                    }
                }

                //Add Optionsets to Holding Solutions
                foreach (var optionset in retrieveAllOptionSetsResponse.OptionSetMetadata)
                {
                    //Check if the optionset is already in a holding solution
                    var solutionOptionset = HoldingSolutionComponents.Where(sc => sc.ObjectId == optionset.MetadataId).FirstOrDefault();

                    if (solutionOptionset == null && optionset.IsManaged == false)
                    {
                        _logger.LogInformation($"Solution: {_holding_optionsets.UniqueName}. Including optionset '{optionset.Name}'");
                        AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
                        {
                            ComponentType = (int)componenttypeValues.OptionSet,
                            ComponentId = optionset.MetadataId.Value,
                            SolutionUniqueName = _holding_optionsets.UniqueName
                        };
                        _organizationService.Execute(addReq);
                    }
                    else
                    {
                        _logger.LogInformation($"Solution: {_holding_optionsets.UniqueName}. Optionset already exists: '{optionset.Name}'");
                    }
                }

                //Add Assemblies to Holding Solutions
                foreach (var pluginAssembly in pluginAssemblies
                        .Where(p => !HoldingSolutionComponents.Select(sc => sc.ObjectId).Contains(p.PluginAssemblyId))
                        .Take(35)
                    )
                {
                    //Check if the plugin is already in a holding solution
                    var solutionAssembly = HoldingSolutionComponents.Where(sc => sc.ObjectId == pluginAssembly.PluginAssemblyId).FirstOrDefault();
                    if (solutionAssembly == null)
                    {
                        _logger.LogInformation($"Solution: {_holding_assemblies.UniqueName}. Including Assembly '{pluginAssembly.Name}'");
                        AddSolutionComponentRequest addReq = new AddSolutionComponentRequest()
                        {
                            ComponentType = (int)componenttypeValues.PluginAssembly,
                            ComponentId = pluginAssembly.PluginAssemblyId.Value,
                            SolutionUniqueName = _holding_assemblies.UniqueName
                        };
                        _organizationService.Execute(addReq);
                    }
                    else
                    {
                        _logger.LogInformation($"Solution: {_holding_assemblies.UniqueName}. Assembly already exists: '{pluginAssembly.Name}'");
                    }
                }
            }
        }

        //Add any dependencies to the area solutions such that the area an holding solutions can be cleanly installed into a new environment
        public void UpdateAreaSolutions()
        {
            InitializeHoldingSolutions();
            using (OrganizationServiceContext xrmContext = new OrganizationServiceContext(_organizationService))
            {
                var solutions = (
                    from s in xrmContext.CreateQuery<Solution>()
                    where s.IsManaged == false
                        && (s.UniqueName.StartsWith("holding_") || s.UniqueName.StartsWith("area_"))
                    select s).ToList();

                int newDependency = 0;
                int existingDependency = 0;
                // For each solution component, retrieve all dependencies for the component.
                foreach (var solution in solutions.Where(s => s.UniqueName.StartsWith("area_")))
                {
                    var solutionComponents = _areaSolutionsComponents.Where(s => s.SolutionId.Id == solution.SolutionId).ToList();

                    // For each solution component, retrieve all dependencies for the component.
                    foreach (var solutionComponent in solutionComponents)
                    {
                        RetrieveRequiredComponentsRequest retrieveDependentComponentRequest = new RetrieveRequiredComponentsRequest
                        {
                            ComponentType = solutionComponent.ComponentType.Value
                            ,
                            ObjectId = solutionComponent.ObjectId.Value
                        };

                        var retrieveDependentComponentRespone = (RetrieveRequiredComponentsResponse)_organizationService.Execute(retrieveDependentComponentRequest);

                        foreach (var untypedDependency in retrieveDependentComponentRespone.EntityCollection.Entities)
                        {
                            Dependency dependency = untypedDependency.ToEntity<Dependency>();

                            //Do we already have the dependent component
                            var existingComponent = HoldingSolutionComponents.Where(
                                sc => sc.ObjectId == dependency.RequiredComponentObjectId || sc.ObjectId == dependency.RequiredComponentParentId).FirstOrDefault();

                            if (existingComponent == null)
                            {
                                newDependency++;

                                switch (dependency.DependentComponentType.Value)
                                {
                                    //Dependent Entity
                                    case (int)componenttypeValues.Entity:
                                        break;

                                    //Other dependencies
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                existingDependency++;
                                _logger.LogInformation($"Component Already Tracked: {solutionComponent.ObjectId}");
                            }
                        }
                    }
                }
            }
        }

        public Solution GetOrCreateSolution(string solutionName, string publisherName, string solutionFriendlyName)
        {
            using (OrganizationServiceContext xrmContext = new OrganizationServiceContext(_organizationService))
            {
                var solution = (
                    from p in xrmContext.CreateQuery<Solution>()
                    where p.UniqueName == solutionName
                    select p).FirstOrDefault();

                if (solution != null)
                    return solution;

                var publisher = (
                    from p in xrmContext.CreateQuery<Publisher>()
                    where p.UniqueName == publisherName
                    select p).FirstOrDefault();

                if (publisher == null)
                    throw new ApplicationException($"Publisher with name '{publisherName}' does not exist.");

                //Define a solution
                _logger.LogInformation($"Creating Holding Solution: {solutionName}. {solutionFriendlyName}");
                Solution newSolution = new Solution
                {
                    UniqueName = solutionName,
                    FriendlyName = solutionFriendlyName,
                    PublisherId = publisher.ToEntityReference(),
                    Description = "Holding solution for D365 Entities",
                    Version = "1.0"
                };

                Entity untypedSolution = newSolution.ToEntity<Entity>();
                newSolution.SolutionId = _organizationService.Create(untypedSolution);
                return newSolution;
            }
        }
        #endregion
    }
}
