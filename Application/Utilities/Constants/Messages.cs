namespace Application.Utilities.Constants
{
    public class Messages
    {
        // General Messages
        public const string Success = "Operation successful";
        public const string AddSucceeded = "Add operation successful";
        public const string UpdateSucceeded = "Update operation successful";
        public const string DeleteSucceeded = "Delete operation successful";
        public const string Exist = "Record already exists";
        public const string NotExist = "Record not found";
        public const string SaveFail = "An error occurred during save operation";
        public const string Fail = "An error occurred during operation";
        public const string ValidationFailed = "Validation error";
        public const string IdFail = "Invalid or missing ID";

        // Authentication Messages
        public const string LoginSuccessful = "Login successful";
        public const string LoginFailed = "Login failed";
        public const string LogoutSuccessful = "Logout successful";
        public const string InvalidCredentials = "Invalid email or password";
        public const string UserNotFound = "User not found";
        public const string EmailAlreadyExists = "This email address is already in use";
        public const string RegistrationSuccessful = "Registration successful";
        public const string RegistrationFailed = "Registration failed";
        public const string PasswordMismatch = "Passwords do not match";
        public const string LoginRequired = "You need to login";

        // User Management Messages
        public const string UserCreated = "User created successfully";
        public const string UserUpdated = "User updated successfully";
        public const string UserDeleted = "User deleted successfully";
        public const string UserAssignedToDietitian = "User assigned to dietitian successfully";
        public const string UserRemovedFromDietitian = "User removed from dietitian successfully";

        // Diet Plan Messages
        public const string DietPlanCreated = "Diet plan created successfully";
        public const string DietPlanUpdated = "Diet plan updated successfully";
        public const string DietPlanDeleted = "Diet plan deleted successfully";
        public const string DietPlanNotFound = "Diet plan not found";
        public const string DietPlanActivated = "Diet plan activated";
        public const string DietPlanDeactivated = "Diet plan deactivated";

        // Meal Messages
        public const string MealCreated = "Meal created successfully";
        public const string MealUpdated = "Meal updated successfully";
        public const string MealDeleted = "Meal deleted successfully";
        public const string MealNotFound = "Meal not found";

        // Authorization Messages
        public const string AccessDenied = "Access denied";
        public const string InsufficientPermissions = "Insufficient permissions";
        public const string AdminRoleRequired = "Admin role required";
        public const string DietitianRoleRequired = "Dietitian role required";

        // Validation Messages
        public const string EmailRequired = "Email address is required";
        public const string PhoneRequired = "Phone number is required";
        public const string InvalidEmail = "Invalid email format";
        public const string InvalidPhoneNumber = "Invalid phone number format";
        public const string PasswordTooWeak = "Password is too weak";
        public const string RequiredFieldMissing = "Required field is missing";

        // Business Logic Messages
        public const string ClientAlreadyHasDietitian = "Client already has a dietitian";
        public const string DietitianCannotBeClient = "Dietitian cannot be a client at the same time";
        public const string InvalidDateRange = "Invalid date range";
        public const string PlanAlreadyExists = "A plan already exists for this date range";
    }
}