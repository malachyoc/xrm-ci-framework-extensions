//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xrm.Typed.Entities
{
	
	
	/// <summary>
	/// A component dependency in CRM.
	/// </summary>
	public partial class Dependency
	{
		
		public enum dependencytypeValues
		{
			
			[System.ComponentModel.Description("None")]
			None = 0,
			
			[System.ComponentModel.Description("Solution Internal")]
			SolutionInternal = 1,
			
			[System.ComponentModel.Description("Published")]
			Published = 2,
			
			[System.ComponentModel.Description("Unpublished")]
			Unpublished = 4,
		}
		
		public enum dependentcomponenttypeValues
		{
			
			[System.ComponentModel.Description("Entity")]
			Entity = 1,
			
			[System.ComponentModel.Description("Attribute")]
			Attribute = 2,
			
			[System.ComponentModel.Description("Relationship")]
			Relationship = 3,
			
			[System.ComponentModel.Description("Attribute Picklist Value")]
			AttributePicklistValue = 4,
			
			[System.ComponentModel.Description("Attribute Lookup Value")]
			AttributeLookupValue = 5,
			
			[System.ComponentModel.Description("View Attribute")]
			ViewAttribute = 6,
			
			[System.ComponentModel.Description("Localized Label")]
			LocalizedLabel = 7,
			
			[System.ComponentModel.Description("Relationship Extra Condition")]
			RelationshipExtraCondition = 8,
			
			[System.ComponentModel.Description("Option Set")]
			OptionSet = 9,
			
			[System.ComponentModel.Description("Entity Relationship")]
			EntityRelationship = 10,
			
			[System.ComponentModel.Description("Entity Relationship Role")]
			EntityRelationshipRole = 11,
			
			[System.ComponentModel.Description("Entity Relationship Relationships")]
			EntityRelationshipRelationships = 12,
			
			[System.ComponentModel.Description("Managed Property")]
			ManagedProperty = 13,
			
			[System.ComponentModel.Description("Entity Key")]
			EntityKey = 14,
			
			[System.ComponentModel.Description("Role")]
			Role = 20,
			
			[System.ComponentModel.Description("Role Privilege")]
			RolePrivilege = 21,
			
			[System.ComponentModel.Description("Display String")]
			DisplayString = 22,
			
			[System.ComponentModel.Description("Display String Map")]
			DisplayStringMap = 23,
			
			[System.ComponentModel.Description("Form")]
			Form = 24,
			
			[System.ComponentModel.Description("Organization")]
			Organization = 25,
			
			[System.ComponentModel.Description("Saved Query")]
			SavedQuery = 26,
			
			[System.ComponentModel.Description("Workflow")]
			Workflow = 29,
			
			[System.ComponentModel.Description("Report")]
			Report = 31,
			
			[System.ComponentModel.Description("Report Entity")]
			ReportEntity = 32,
			
			[System.ComponentModel.Description("Report Category")]
			ReportCategory = 33,
			
			[System.ComponentModel.Description("Report Visibility")]
			ReportVisibility = 34,
			
			[System.ComponentModel.Description("Attachment")]
			Attachment = 35,
			
			[System.ComponentModel.Description("Email Template")]
			EmailTemplate = 36,
			
			[System.ComponentModel.Description("Contract Template")]
			ContractTemplate = 37,
			
			[System.ComponentModel.Description("KB Article Template")]
			KBArticleTemplate = 38,
			
			[System.ComponentModel.Description("Mail Merge Template")]
			MailMergeTemplate = 39,
			
			[System.ComponentModel.Description("Duplicate Rule")]
			DuplicateRule = 44,
			
			[System.ComponentModel.Description("Duplicate Rule Condition")]
			DuplicateRuleCondition = 45,
			
			[System.ComponentModel.Description("Entity Map")]
			EntityMap = 46,
			
			[System.ComponentModel.Description("Attribute Map")]
			AttributeMap = 47,
			
			[System.ComponentModel.Description("Ribbon Command")]
			RibbonCommand = 48,
			
			[System.ComponentModel.Description("Ribbon Context Group")]
			RibbonContextGroup = 49,
			
			[System.ComponentModel.Description("Ribbon Customization")]
			RibbonCustomization = 50,
			
			[System.ComponentModel.Description("Ribbon Rule")]
			RibbonRule = 52,
			
			[System.ComponentModel.Description("Ribbon Tab To Command Map")]
			RibbonTabToCommandMap = 53,
			
			[System.ComponentModel.Description("Ribbon Diff")]
			RibbonDiff = 55,
			
			[System.ComponentModel.Description("Saved Query Visualization")]
			SavedQueryVisualization = 59,
			
			[System.ComponentModel.Description("System Form")]
			SystemForm = 60,
			
			[System.ComponentModel.Description("Web Resource")]
			WebResource = 61,
			
			[System.ComponentModel.Description("Site Map")]
			SiteMap = 62,
			
			[System.ComponentModel.Description("Connection Role")]
			ConnectionRole = 63,
			
			[System.ComponentModel.Description("Field Security Profile")]
			FieldSecurityProfile = 70,
			
			[System.ComponentModel.Description("Field Permission")]
			FieldPermission = 71,
			
			[System.ComponentModel.Description("Plugin Type")]
			PluginType = 90,
			
			[System.ComponentModel.Description("Plugin Assembly")]
			PluginAssembly = 91,
			
			[System.ComponentModel.Description("SDK Message Processing Step")]
			SDKMessageProcessingStep = 92,
			
			[System.ComponentModel.Description("SDK Message Processing Step Image")]
			SDKMessageProcessingStepImage = 93,
			
			[System.ComponentModel.Description("Service Endpoint")]
			ServiceEndpoint = 95,
			
			[System.ComponentModel.Description("Routing Rule")]
			RoutingRule = 150,
			
			[System.ComponentModel.Description("Routing Rule Item")]
			RoutingRuleItem = 151,
			
			[System.ComponentModel.Description("SLA")]
			SLA = 152,
			
			[System.ComponentModel.Description("SLA Item")]
			SLAItem = 153,
			
			[System.ComponentModel.Description("Convert Rule")]
			ConvertRule = 154,
			
			[System.ComponentModel.Description("Convert Rule Item")]
			ConvertRuleItem = 155,
			
			[System.ComponentModel.Description("Hierarchy Rule")]
			HierarchyRule = 65,
			
			[System.ComponentModel.Description("Mobile Offline Profile")]
			MobileOfflineProfile = 161,
			
			[System.ComponentModel.Description("Mobile Offline Profile Item")]
			MobileOfflineProfileItem = 162,
			
			[System.ComponentModel.Description("Similarity Rule")]
			SimilarityRule = 165,
			
			[System.ComponentModel.Description("Custom Control")]
			CustomControl = 66,
			
			[System.ComponentModel.Description("Custom Control Default Config")]
			CustomControlDefaultConfig = 68,
		}
		
		public enum requiredcomponenttypeValues
		{
			
			[System.ComponentModel.Description("Entity")]
			Entity = 1,
			
			[System.ComponentModel.Description("Attribute")]
			Attribute = 2,
			
			[System.ComponentModel.Description("Relationship")]
			Relationship = 3,
			
			[System.ComponentModel.Description("Attribute Picklist Value")]
			AttributePicklistValue = 4,
			
			[System.ComponentModel.Description("Attribute Lookup Value")]
			AttributeLookupValue = 5,
			
			[System.ComponentModel.Description("View Attribute")]
			ViewAttribute = 6,
			
			[System.ComponentModel.Description("Localized Label")]
			LocalizedLabel = 7,
			
			[System.ComponentModel.Description("Relationship Extra Condition")]
			RelationshipExtraCondition = 8,
			
			[System.ComponentModel.Description("Option Set")]
			OptionSet = 9,
			
			[System.ComponentModel.Description("Entity Relationship")]
			EntityRelationship = 10,
			
			[System.ComponentModel.Description("Entity Relationship Role")]
			EntityRelationshipRole = 11,
			
			[System.ComponentModel.Description("Entity Relationship Relationships")]
			EntityRelationshipRelationships = 12,
			
			[System.ComponentModel.Description("Managed Property")]
			ManagedProperty = 13,
			
			[System.ComponentModel.Description("Entity Key")]
			EntityKey = 14,
			
			[System.ComponentModel.Description("Role")]
			Role = 20,
			
			[System.ComponentModel.Description("Role Privilege")]
			RolePrivilege = 21,
			
			[System.ComponentModel.Description("Display String")]
			DisplayString = 22,
			
			[System.ComponentModel.Description("Display String Map")]
			DisplayStringMap = 23,
			
			[System.ComponentModel.Description("Form")]
			Form = 24,
			
			[System.ComponentModel.Description("Organization")]
			Organization = 25,
			
			[System.ComponentModel.Description("Saved Query")]
			SavedQuery = 26,
			
			[System.ComponentModel.Description("Workflow")]
			Workflow = 29,
			
			[System.ComponentModel.Description("Report")]
			Report = 31,
			
			[System.ComponentModel.Description("Report Entity")]
			ReportEntity = 32,
			
			[System.ComponentModel.Description("Report Category")]
			ReportCategory = 33,
			
			[System.ComponentModel.Description("Report Visibility")]
			ReportVisibility = 34,
			
			[System.ComponentModel.Description("Attachment")]
			Attachment = 35,
			
			[System.ComponentModel.Description("Email Template")]
			EmailTemplate = 36,
			
			[System.ComponentModel.Description("Contract Template")]
			ContractTemplate = 37,
			
			[System.ComponentModel.Description("KB Article Template")]
			KBArticleTemplate = 38,
			
			[System.ComponentModel.Description("Mail Merge Template")]
			MailMergeTemplate = 39,
			
			[System.ComponentModel.Description("Duplicate Rule")]
			DuplicateRule = 44,
			
			[System.ComponentModel.Description("Duplicate Rule Condition")]
			DuplicateRuleCondition = 45,
			
			[System.ComponentModel.Description("Entity Map")]
			EntityMap = 46,
			
			[System.ComponentModel.Description("Attribute Map")]
			AttributeMap = 47,
			
			[System.ComponentModel.Description("Ribbon Command")]
			RibbonCommand = 48,
			
			[System.ComponentModel.Description("Ribbon Context Group")]
			RibbonContextGroup = 49,
			
			[System.ComponentModel.Description("Ribbon Customization")]
			RibbonCustomization = 50,
			
			[System.ComponentModel.Description("Ribbon Rule")]
			RibbonRule = 52,
			
			[System.ComponentModel.Description("Ribbon Tab To Command Map")]
			RibbonTabToCommandMap = 53,
			
			[System.ComponentModel.Description("Ribbon Diff")]
			RibbonDiff = 55,
			
			[System.ComponentModel.Description("Saved Query Visualization")]
			SavedQueryVisualization = 59,
			
			[System.ComponentModel.Description("System Form")]
			SystemForm = 60,
			
			[System.ComponentModel.Description("Web Resource")]
			WebResource = 61,
			
			[System.ComponentModel.Description("Site Map")]
			SiteMap = 62,
			
			[System.ComponentModel.Description("Connection Role")]
			ConnectionRole = 63,
			
			[System.ComponentModel.Description("Field Security Profile")]
			FieldSecurityProfile = 70,
			
			[System.ComponentModel.Description("Field Permission")]
			FieldPermission = 71,
			
			[System.ComponentModel.Description("Plugin Type")]
			PluginType = 90,
			
			[System.ComponentModel.Description("Plugin Assembly")]
			PluginAssembly = 91,
			
			[System.ComponentModel.Description("SDK Message Processing Step")]
			SDKMessageProcessingStep = 92,
			
			[System.ComponentModel.Description("SDK Message Processing Step Image")]
			SDKMessageProcessingStepImage = 93,
			
			[System.ComponentModel.Description("Service Endpoint")]
			ServiceEndpoint = 95,
			
			[System.ComponentModel.Description("Routing Rule")]
			RoutingRule = 150,
			
			[System.ComponentModel.Description("Routing Rule Item")]
			RoutingRuleItem = 151,
			
			[System.ComponentModel.Description("SLA")]
			SLA = 152,
			
			[System.ComponentModel.Description("SLA Item")]
			SLAItem = 153,
			
			[System.ComponentModel.Description("Convert Rule")]
			ConvertRule = 154,
			
			[System.ComponentModel.Description("Convert Rule Item")]
			ConvertRuleItem = 155,
			
			[System.ComponentModel.Description("Hierarchy Rule")]
			HierarchyRule = 65,
			
			[System.ComponentModel.Description("Mobile Offline Profile")]
			MobileOfflineProfile = 161,
			
			[System.ComponentModel.Description("Mobile Offline Profile Item")]
			MobileOfflineProfileItem = 162,
			
			[System.ComponentModel.Description("Similarity Rule")]
			SimilarityRule = 165,
			
			[System.ComponentModel.Description("Custom Control")]
			CustomControl = 66,
			
			[System.ComponentModel.Description("Custom Control Default Config")]
			CustomControlDefaultConfig = 68,
		}
	}
	
	/// <summary>
	/// Assembly that contains one or more plug-in types.
	/// </summary>
	public partial class PluginAssembly
	{
		
		public enum componentstateValues
		{
			
			[System.ComponentModel.Description("Published")]
			Published = 0,
			
			[System.ComponentModel.Description("Unpublished")]
			Unpublished = 1,
			
			[System.ComponentModel.Description("Deleted")]
			Deleted = 2,
			
			[System.ComponentModel.Description("Deleted Unpublished")]
			DeletedUnpublished = 3,
		}
		
		#region 
	static
		public class ismanagedValues
		{
			
			[System.ComponentModel.Description("Unmanaged")]
			public const bool Unmanaged = false;
			
			[System.ComponentModel.Description("Managed")]
			public const bool Managed = true;
		}
		#endregion
		
		public enum isolationmodeValues
		{
			
			[System.ComponentModel.Description("None")]
			None = 1,
			
			[System.ComponentModel.Description("Sandbox")]
			Sandbox = 2,
		}
		
		public enum sourcetypeValues
		{
			
			[System.ComponentModel.Description("Database")]
			Database = 0,
			
			[System.ComponentModel.Description("Disk")]
			Disk = 1,
			
			[System.ComponentModel.Description("Normal")]
			Normal = 2,
		}
	}
	
	/// <summary>
	/// Type that inherits from the IPlugin interface and is contained within a plug-in assembly.
	/// </summary>
	public partial class PluginType
	{
		
		public enum componentstateValues
		{
			
			[System.ComponentModel.Description("Published")]
			Published = 0,
			
			[System.ComponentModel.Description("Unpublished")]
			Unpublished = 1,
			
			[System.ComponentModel.Description("Deleted")]
			Deleted = 2,
			
			[System.ComponentModel.Description("Deleted Unpublished")]
			DeletedUnpublished = 3,
		}
		
		#region 
	static
		public class ismanagedValues
		{
			
			[System.ComponentModel.Description("Unmanaged")]
			public const bool Unmanaged = false;
			
			[System.ComponentModel.Description("Managed")]
			public const bool Managed = true;
		}
		#endregion
		
		#region 
	static
		public class isworkflowactivityValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
	}
	
	/// <summary>
	/// A publisher of a CRM solution.
	/// </summary>
	public partial class Publisher
	{
		
		public enum address1_addresstypecodeValues
		{
			
			[System.ComponentModel.Description("Default Value")]
			DefaultValue = 1,
		}
		
		public enum address1_shippingmethodcodeValues
		{
			
			[System.ComponentModel.Description("Default Value")]
			DefaultValue = 1,
		}
		
		public enum address2_addresstypecodeValues
		{
			
			[System.ComponentModel.Description("Default Value")]
			DefaultValue = 1,
		}
		
		public enum address2_shippingmethodcodeValues
		{
			
			[System.ComponentModel.Description("Default Value")]
			DefaultValue = 1,
		}
		
		#region 
	static
		public class isreadonlyValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
	}
	
	/// <summary>
	/// A solution which contains CRM customizations.
	/// </summary>
	public partial class Solution
	{
		
		#region 
	static
		public class isinternalValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
		
		#region 
	static
		public class ismanagedValues
		{
			
			[System.ComponentModel.Description("Unmanaged")]
			public const bool Unmanaged = false;
			
			[System.ComponentModel.Description("Managed")]
			public const bool Managed = true;
		}
		#endregion
		
		#region 
	static
		public class isvisibleValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
		
		public enum solutiontypeValues
		{
			
			[System.ComponentModel.Description("None")]
			None = 0,
			
			[System.ComponentModel.Description("Snapshot")]
			Snapshot = 1,
			
			[System.ComponentModel.Description("Internal")]
			Internal = 2,
		}
	}
	
	/// <summary>
	/// A component of a CRM solution.
	/// </summary>
	public partial class SolutionComponent
	{
		
		public enum componenttypeValues
		{
			
			[System.ComponentModel.Description("Entity")]
			Entity = 1,
			
			[System.ComponentModel.Description("Attribute")]
			Attribute = 2,
			
			[System.ComponentModel.Description("Relationship")]
			Relationship = 3,
			
			[System.ComponentModel.Description("Attribute Picklist Value")]
			AttributePicklistValue = 4,
			
			[System.ComponentModel.Description("Attribute Lookup Value")]
			AttributeLookupValue = 5,
			
			[System.ComponentModel.Description("View Attribute")]
			ViewAttribute = 6,
			
			[System.ComponentModel.Description("Localized Label")]
			LocalizedLabel = 7,
			
			[System.ComponentModel.Description("Relationship Extra Condition")]
			RelationshipExtraCondition = 8,
			
			[System.ComponentModel.Description("Option Set")]
			OptionSet = 9,
			
			[System.ComponentModel.Description("Entity Relationship")]
			EntityRelationship = 10,
			
			[System.ComponentModel.Description("Entity Relationship Role")]
			EntityRelationshipRole = 11,
			
			[System.ComponentModel.Description("Entity Relationship Relationships")]
			EntityRelationshipRelationships = 12,
			
			[System.ComponentModel.Description("Managed Property")]
			ManagedProperty = 13,
			
			[System.ComponentModel.Description("Entity Key")]
			EntityKey = 14,
			
			[System.ComponentModel.Description("Role")]
			Role = 20,
			
			[System.ComponentModel.Description("Role Privilege")]
			RolePrivilege = 21,
			
			[System.ComponentModel.Description("Display String")]
			DisplayString = 22,
			
			[System.ComponentModel.Description("Display String Map")]
			DisplayStringMap = 23,
			
			[System.ComponentModel.Description("Form")]
			Form = 24,
			
			[System.ComponentModel.Description("Organization")]
			Organization = 25,
			
			[System.ComponentModel.Description("Saved Query")]
			SavedQuery = 26,
			
			[System.ComponentModel.Description("Workflow")]
			Workflow = 29,
			
			[System.ComponentModel.Description("Report")]
			Report = 31,
			
			[System.ComponentModel.Description("Report Entity")]
			ReportEntity = 32,
			
			[System.ComponentModel.Description("Report Category")]
			ReportCategory = 33,
			
			[System.ComponentModel.Description("Report Visibility")]
			ReportVisibility = 34,
			
			[System.ComponentModel.Description("Attachment")]
			Attachment = 35,
			
			[System.ComponentModel.Description("Email Template")]
			EmailTemplate = 36,
			
			[System.ComponentModel.Description("Contract Template")]
			ContractTemplate = 37,
			
			[System.ComponentModel.Description("KB Article Template")]
			KBArticleTemplate = 38,
			
			[System.ComponentModel.Description("Mail Merge Template")]
			MailMergeTemplate = 39,
			
			[System.ComponentModel.Description("Duplicate Rule")]
			DuplicateRule = 44,
			
			[System.ComponentModel.Description("Duplicate Rule Condition")]
			DuplicateRuleCondition = 45,
			
			[System.ComponentModel.Description("Entity Map")]
			EntityMap = 46,
			
			[System.ComponentModel.Description("Attribute Map")]
			AttributeMap = 47,
			
			[System.ComponentModel.Description("Ribbon Command")]
			RibbonCommand = 48,
			
			[System.ComponentModel.Description("Ribbon Context Group")]
			RibbonContextGroup = 49,
			
			[System.ComponentModel.Description("Ribbon Customization")]
			RibbonCustomization = 50,
			
			[System.ComponentModel.Description("Ribbon Rule")]
			RibbonRule = 52,
			
			[System.ComponentModel.Description("Ribbon Tab To Command Map")]
			RibbonTabToCommandMap = 53,
			
			[System.ComponentModel.Description("Ribbon Diff")]
			RibbonDiff = 55,
			
			[System.ComponentModel.Description("Saved Query Visualization")]
			SavedQueryVisualization = 59,
			
			[System.ComponentModel.Description("System Form")]
			SystemForm = 60,
			
			[System.ComponentModel.Description("Web Resource")]
			WebResource = 61,
			
			[System.ComponentModel.Description("Site Map")]
			SiteMap = 62,
			
			[System.ComponentModel.Description("Connection Role")]
			ConnectionRole = 63,
			
			[System.ComponentModel.Description("Field Security Profile")]
			FieldSecurityProfile = 70,
			
			[System.ComponentModel.Description("Field Permission")]
			FieldPermission = 71,
			
			[System.ComponentModel.Description("Plugin Type")]
			PluginType = 90,
			
			[System.ComponentModel.Description("Plugin Assembly")]
			PluginAssembly = 91,
			
			[System.ComponentModel.Description("SDK Message Processing Step")]
			SDKMessageProcessingStep = 92,
			
			[System.ComponentModel.Description("SDK Message Processing Step Image")]
			SDKMessageProcessingStepImage = 93,
			
			[System.ComponentModel.Description("Service Endpoint")]
			ServiceEndpoint = 95,
			
			[System.ComponentModel.Description("Routing Rule")]
			RoutingRule = 150,
			
			[System.ComponentModel.Description("Routing Rule Item")]
			RoutingRuleItem = 151,
			
			[System.ComponentModel.Description("SLA")]
			SLA = 152,
			
			[System.ComponentModel.Description("SLA Item")]
			SLAItem = 153,
			
			[System.ComponentModel.Description("Convert Rule")]
			ConvertRule = 154,
			
			[System.ComponentModel.Description("Convert Rule Item")]
			ConvertRuleItem = 155,
			
			[System.ComponentModel.Description("Hierarchy Rule")]
			HierarchyRule = 65,
			
			[System.ComponentModel.Description("Mobile Offline Profile")]
			MobileOfflineProfile = 161,
			
			[System.ComponentModel.Description("Mobile Offline Profile Item")]
			MobileOfflineProfileItem = 162,
			
			[System.ComponentModel.Description("Similarity Rule")]
			SimilarityRule = 165,
			
			[System.ComponentModel.Description("Custom Control")]
			CustomControl = 66,
			
			[System.ComponentModel.Description("Custom Control Default Config")]
			CustomControlDefaultConfig = 68,
		}
		
		#region 
	static
		public class ismetadataValues
		{
			
			[System.ComponentModel.Description("Data")]
			public const bool Data = false;
			
			[System.ComponentModel.Description("Metadata")]
			public const bool Metadata = true;
		}
		#endregion
		
		public enum rootcomponentbehaviorValues
		{
			
			[System.ComponentModel.Description("Include Subcomponents")]
			IncludeSubcomponents = 0,
			
			[System.ComponentModel.Description("Do not include subcomponents")]
			Donotincludesubcomponents = 1,
			
			[System.ComponentModel.Description("Include As Shell Only")]
			IncludeAsShellOnly = 2,
		}
	}
	
	/// <summary>
	/// Data equivalent to files used in Web development. Web resources provide client-side components that are used to provide custom user interface elements.
	/// </summary>
	public partial class WebResource
	{
		
		public enum componentstateValues
		{
			
			[System.ComponentModel.Description("Published")]
			Published = 0,
			
			[System.ComponentModel.Description("Unpublished")]
			Unpublished = 1,
			
			[System.ComponentModel.Description("Deleted")]
			Deleted = 2,
			
			[System.ComponentModel.Description("Deleted Unpublished")]
			DeletedUnpublished = 3,
		}
		
		#region 
	static
		public class isavailableformobileofflineValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
		
		#region 
	static
		public class isenabledformobileclientValues
		{
			
			[System.ComponentModel.Description("No")]
			public const bool No = false;
			
			[System.ComponentModel.Description("Yes")]
			public const bool Yes = true;
		}
		#endregion
		
		#region 
	static
		public class ismanagedValues
		{
			
			[System.ComponentModel.Description("Unmanaged")]
			public const bool Unmanaged = false;
			
			[System.ComponentModel.Description("Managed")]
			public const bool Managed = true;
		}
		#endregion
		
		public enum webresourcetypeValues
		{
			
			[System.ComponentModel.Description("Webpage (HTML)")]
			WebpageHTML = 1,
			
			[System.ComponentModel.Description("Style Sheet (CSS)")]
			StyleSheetCSS = 2,
			
			[System.ComponentModel.Description("Script (JScript)")]
			ScriptJScript = 3,
			
			[System.ComponentModel.Description("Data (XML)")]
			DataXML = 4,
			
			[System.ComponentModel.Description("PNG format")]
			PNGformat = 5,
			
			[System.ComponentModel.Description("JPG format")]
			JPGformat = 6,
			
			[System.ComponentModel.Description("GIF format")]
			GIFformat = 7,
			
			[System.ComponentModel.Description("Silverlight (XAP)")]
			SilverlightXAP = 8,
			
			[System.ComponentModel.Description("Style Sheet (XSL)")]
			StyleSheetXSL = 9,
			
			[System.ComponentModel.Description("ICO format")]
			ICOformat = 10,
		}
	}
}
