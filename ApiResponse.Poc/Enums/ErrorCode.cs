namespace ApiResponse.Poc;

/// <summary>
/// Represent, application error codes
/// </summary>
public enum ErrorCode
{
    Unknown,
    ValidationFailed,
    InvalidPermission,
    InvalidToken,
    InvalidData,
    EmailUnconfirmed,
    EntityDoesNotExist,
    ConfirmationTokenInvalid,
    EmailAlreadyTaken,
    FailedIdentity,
    EmailAlreadyConfirmed,
    IncorrectPassword,
    ServiceNotProvided,
    DatabaseError
}