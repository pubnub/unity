using System;
using System.Collections.Generic;

namespace PubNubAPI
{
    public class PNSendFileInputsToS3
    {
        public PNFileData Data;
	    public PNFileUploadRequest FileUploadRequest;

    }

    public class PNFileData
    {
        public string ID; 

    }

    // PNFileUploadRequest is used to store the info related to file upload to S3
    public class PNFileUploadRequest{
        public string URL;
        public string Method;
        public List<PNFormField> FormFields;
    }

    // PNFormField is part of the struct used in file upload to S3
    public class PNFormField{
        public string Key;
        public string Value;
    }

    // PNFileDetails is used in the responses to show File Info
    public class PNFileDetails{
        public string Name;
        public string ID;
        public string URL;
    }

    // PNFileMessageAndDetails is used to store the file message and file info

    public class PNFileMessageAndDetails{
        public PNPublishMessage PNMessage;
        public PNFileDetails PNFile;
    }

    // PNPublishMessage is the part of the message struct used in Publish File
    public class PNPublishMessage{
        public string Text;
    }

    public class PNFileInfoForPublish
    {
        public string Name;
        public string ID;
    }
}
