using System;

namespace PubNubAPI
{
    public enum PNStatusCategory
    {
        PNUnknownCategory,
        PNAcknowledgmentCategory,
        PNAccessDeniedCategory,
        PNTimeoutCategory,
        PNNetworkIssuesCategory,
        PNConnectedCategory,
        PNReconnectedCategory,
        PNDisconnectedCategory,
        PNUnexpectedDisconnectCategory,
        PNCancelledCategory,
        PNBadRequestCategory,
        PNMalformedFilterExpressionCategory,
        PNMalformedResponseCategory,
        PNDecryptionErrorCategory,
        PNTLSConnectionFailedCategory,
        PNTLSUntrustedCertificateCategory,

        PNRequestMessageCountExceededCategory,
        PNReconnectionAttemptsExhausted


    }
}