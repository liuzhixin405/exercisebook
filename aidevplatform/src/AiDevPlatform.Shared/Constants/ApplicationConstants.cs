namespace AiDevPlatform.Shared.Constants;

public static class ApplicationConstants
{
    public const string DefaultApplicationName = "AI 开发平台";
    public const string DefaultApplicationCode = "LOWCODE_PLATFORM";
    public const string DefaultApplicationVersion = "1.0.0";

    public const string DefaultOrganizationName = "默认组织";
    public const string DefaultOrganizationCode = "DEFAULT_ORG";

    public const string DefaultAdminRoleName = "系统管理员";
    public const string DefaultAdminRoleCode = "SYSTEM_ADMIN";

    public const string DefaultUserRoleName = "普通用户";
    public const string DefaultUserRoleCode = "USER";

    public const string DefaultDeveloperRoleName = "开发者";
    public const string DefaultDeveloperRoleCode = "DEVELOPER";

    public const string DefaultDesignerRoleName = "设计师";
    public const string DefaultDesignerRoleCode = "DESIGNER";
}

public static class PermissionConstants
{
    public const string ViewApplication = "APPLICATION_VIEW";
    public const string CreateApplication = "APPLICATION_CREATE";
    public const string UpdateApplication = "APPLICATION_UPDATE";
    public const string DeleteApplication = "APPLICATION_DELETE";
    public const string PublishApplication = "APPLICATION_PUBLISH";

    public const string ViewForm = "FORM_VIEW";
    public const string CreateForm = "FORM_CREATE";
    public const string UpdateForm = "FORM_UPDATE";
    public const string DeleteForm = "FORM_DELETE";
    public const string SubmitForm = "FORM_SUBMIT";

    public const string ViewWorkflow = "WORKFLOW_VIEW";
    public const string CreateWorkflow = "WORKFLOW_CREATE";
    public const string UpdateWorkflow = "WORKFLOW_UPDATE";
    public const string DeleteWorkflow = "WORKFLOW_DELETE";
    public const string ExecuteWorkflow = "WORKFLOW_EXECUTE";

    public const string ViewReport = "REPORT_VIEW";
    public const string CreateReport = "REPORT_CREATE";
    public const string UpdateReport = "REPORT_UPDATE";
    public const string DeleteReport = "REPORT_DELETE";
    public const string ExecuteReport = "REPORT_EXECUTE";
    public const string ExportReport = "REPORT_EXPORT";

    public const string ViewDataModel = "DATAMODEL_VIEW";
    public const string CreateDataModel = "DATAMODEL_CREATE";
    public const string UpdateDataModel = "DATAMODEL_UPDATE";
    public const string DeleteDataModel = "DATAMODEL_DELETE";

    public const string ViewComponent = "COMPONENT_VIEW";
    public const string CreateComponent = "COMPONENT_CREATE";
    public const string UpdateComponent = "COMPONENT_UPDATE";
    public const string DeleteComponent = "COMPONENT_DELETE";

    public const string ViewUser = "USER_VIEW";
    public const string CreateUser = "USER_CREATE";
    public const string UpdateUser = "USER_UPDATE";
    public const string DeleteUser = "USER_DELETE";

    public const string ViewOrganization = "ORGANIZATION_VIEW";
    public const string CreateOrganization = "ORGANIZATION_CREATE";
    public const string UpdateOrganization = "ORGANIZATION_UPDATE";
    public const string DeleteOrganization = "ORGANIZATION_DELETE";
}

public static class ComponentConstants
{
    public const string BasicButton = "BASIC_BUTTON";
    public const string BasicInput = "BASIC_INPUT";
    public const string BasicTextarea = "BASIC_TEXTAREA";
    public const string BasicSelect = "BASIC_SELECT";
    public const string BasicCheckbox = "BASIC_CHECKBOX";
    public const string BasicRadio = "BASIC_RADIO";
    public const string BasicDatePicker = "BASIC_DATE_PICKER";
    public const string BasicTimePicker = "BASIC_TIME_PICKER";
    public const string BasicFileUpload = "BASIC_FILE_UPLOAD";
    public const string BasicImageUpload = "BASIC_IMAGE_UPLOAD";

    public const string LayoutContainer = "LAYOUT_CONTAINER";
    public const string LayoutRow = "LAYOUT_ROW";
    public const string LayoutCol = "LAYOUT_COL";
    public const string LayoutCard = "LAYOUT_CARD";
    public const string LayoutTabs = "LAYOUT_TABS";
    public const string LayoutAccordion = "LAYOUT_ACCORDION";
    public const string LayoutModal = "LAYOUT_MODAL";
    public const string LayoutDrawer = "LAYOUT_DRAWER";

    public const string DataTable = "DATA_TABLE";
    public const string DataGrid = "DATA_GRID";
    public const string DataList = "DATA_LIST";
    public const string DataTree = "DATA_TREE";
    public const string DataPagination = "DATA_PAGINATION";
    public const string DataSearch = "DATA_SEARCH";
    public const string DataFilter = "DATA_FILTER";

    public const string ChartLine = "CHART_LINE";
    public const string ChartBar = "CHART_BAR";
    public const string ChartPie = "CHART_PIE";
    public const string ChartArea = "CHART_AREA";
    public const string ChartScatter = "CHART_SCATTER";
    public const string ChartGauge = "CHART_GAUGE";
    public const string ChartFunnel = "CHART_FUNNEL";
    public const string ChartRadar = "CHART_RADAR";

    public const string NavigationMenu = "NAVIGATION_MENU";
    public const string NavigationBreadcrumb = "NAVIGATION_BREADCRUMB";
    public const string NavigationSteps = "NAVIGATION_STEPS";
    public const string NavigationPagination = "NAVIGATION_PAGINATION";

    public const string MediaImage = "MEDIA_IMAGE";
    public const string MediaVideo = "MEDIA_VIDEO";
    public const string MediaAudio = "MEDIA_AUDIO";
    public const string MediaCarousel = "MEDIA_CAROUSEL";

    public const string CalendarBasic = "CALENDAR_BASIC";
    public const string CalendarSchedule = "CALENDAR_SCHEDULE";
    public const string CalendarTimeline = "CALENDAR_TIMELINE";

    public const string MapBasic = "MAP_BASIC";
    public const string MapMarker = "MAP_MARKER";
    public const string MapRoute = "MAP_ROUTE";
    public const string MapHeatmap = "MAP_HEATMAP";
}

public static class WorkflowConstants
{
    public const string StartEvent = "START_EVENT";
    public const string EndEvent = "END_EVENT";
    public const string UserTask = "USER_TASK";
    public const string ServiceTask = "SERVICE_TASK";
    public const string ScriptTask = "SCRIPT_TASK";
    public const string TimerEvent = "TIMER_EVENT";
    public const string MessageEvent = "MESSAGE_EVENT";
    public const string ConditionalGateway = "CONDITIONAL_GATEWAY";
    public const string ParallelGateway = "PARALLEL_GATEWAY";
    public const string ExclusiveGateway = "EXCLUSIVE_GATEWAY";
    public const string InclusiveGateway = "INCLUSIVE_GATEWAY";
    public const string SubProcess = "SUB_PROCESS";
    public const string CallActivity = "CALL_ACTIVITY";
}

public static class DataTypeConstants
{
    public const string String = "STRING";
    public const string Integer = "INTEGER";
    public const string Long = "LONG";
    public const string Decimal = "DECIMAL";
    public const string Boolean = "BOOLEAN";
    public const string DateTime = "DATETIME";
    public const string Date = "DATE";
    public const string Time = "TIME";
    public const string Guid = "GUID";
    public const string Binary = "BINARY";
    public const string Json = "JSON";
    public const string Xml = "XML";
    public const string Email = "EMAIL";
    public const string Phone = "PHONE";
    public const string Url = "URL";
    public const string Color = "COLOR";
    public const string File = "FILE";
    public const string Image = "IMAGE";
}

public static class ValidationConstants
{
    public const string Required = "REQUIRED";
    public const string MinLength = "MIN_LENGTH";
    public const string MaxLength = "MAX_LENGTH";
    public const string MinValue = "MIN_VALUE";
    public const string MaxValue = "MAX_VALUE";
    public const string Pattern = "PATTERN";
    public const string Email = "EMAIL";
    public const string Phone = "PHONE";
    public const string Url = "URL";
    public const string Date = "DATE";
    public const string Time = "TIME";
    public const string DateTime = "DATETIME";
    public const string Number = "NUMBER";
    public const string Integer = "INTEGER";
    public const string Decimal = "DECIMAL";
    public const string Boolean = "BOOLEAN";
    public const string Guid = "GUID";
    public const string File = "FILE";
    public const string Image = "IMAGE";
    public const string Custom = "CUSTOM";
}
