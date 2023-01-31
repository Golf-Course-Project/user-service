namespace UserService.Enums
{
    public enum ApiMessageCodes
    {
        Success = 200,
        Created = 201,
        Updated = 202,
        Deleted = 203,
        Destroyed = 204,
        NoResults = 300,
        NotFound = 400,
        InvalidModelState = 402,
        EmptyValue = 403,
        Expired = 405,
        AlreadyExists = 406,
        NullValue = 407,
        NotOkay = 408,
        BlogStorageFailure = 409,
        Failed = 600,
        InvalidParamValue = 601,
        Throttled = 602,
        FailedToSendMessage = 603,
        PartialFailure = 604,
        ExceptionThrown = 501,
        AuthFailed = 500
    }

    public enum AuthMessageCodes
    {
        Success = 200,
        InvalidFormat = 400,
        NotFound = 404,
        Blocked = 401,
        LoginAttempts = 402,
        NotConfirmed = 403,
        InvalidStatus = 405,
        NotActive = 406,
        InvalidLogin = 500,
        Failed = 501,
        ExceptionThrown = 502
    }
}
