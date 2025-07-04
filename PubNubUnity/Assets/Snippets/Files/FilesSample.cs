// snippet.using
using PubnubApi;
using PubnubApi.Unity;

// snippet.end
using System.Threading.Tasks;
using UnityEngine;
using System;

public class FilesSample
{
    private static Pubnub pubnub;

    static void InitSample()
    {
        // snippet.init
        PNConfiguration pnConfiguration = new PNConfiguration(new UserId("myUniqueUserId"))
        {
            SubscribeKey = "demo",
            PublishKey = "demo",
            Secure = true
        };

        Pubnub pubnub = PubnubUnityUtils.NewUnityPubnub(pnConfiguration);

        // If you're using Unity Editor setup you can get the Pubnub instance from PNManagerBehaviour
        // For more details, see https://www.pubnub.com/docs/sdks/unity#configure-pubnub
        /*
        [SerializeField] private PNManagerBehaviour pubnubManager;
        Pubnub pubnub = pubnubManager.pubnub;
        */

        // snippet.end
    }

    public static async Task ListFilesBasicUsage()
    {
        // snippet.list_files_basic_usage
        PNResult<PNListFilesResult> listFilesResponse = await pubnub.ListFiles()
            .Channel("my_channel")
            .ExecuteAsync();
        PNListFilesResult listFilesResult = listFilesResponse.Result;
        PNStatus listFilesStatus = listFilesResponse.Status;
        if (!listFilesStatus.Error && listFilesResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(listFilesResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(listFilesStatus));
        }
        // snippet.end
    }

    public static async Task GetFileUrlBasicUsage()
    {
        // snippet.get_file_url_basic_usage
        PNResult<PNFileUrlResult> getFileUrlResponse = await pubnub.GetFileUrl()
            .Channel("my_channel")
            .FileId("d9515cb7-48a7-41a4-9284-f4bf331bc770")
            .FileName("cat_picture.jpg")
            .ExecuteAsync();
        PNFileUrlResult getFileUrlResult = getFileUrlResponse.Result;
        PNStatus getFileUrlStatus = getFileUrlResponse.Status;
        if (!getFileUrlStatus.Error && getFileUrlResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(getFileUrlResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(getFileUrlStatus));
        }
        // snippet.end
    }

    public static async Task DownloadFileBasicUsage(string downloadUrlFileName)
    {
        // snippet.download_file_basic_usage
        PNResult<PNDownloadFileResult> fileDownloadResponse = await pubnub.DownloadFile()
            .Channel("my_channel")
            .FileId("d9515cb7-48a7-41a4-9284-f4bf331bc770")
            .FileName("cat_picture.jpg")
            .ExecuteAsync();
        PNDownloadFileResult fileDownloadResult = fileDownloadResponse.Result;
        PNStatus fileDownloadStatus = fileDownloadResponse.Status;
        if (!fileDownloadStatus.Error && fileDownloadResult != null)
        {
            fileDownloadResult.SaveFileToLocal(downloadUrlFileName); //saves to bin folder if no path is provided
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(fileDownloadResult.FileName));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(fileDownloadStatus));
        }
        // snippet.end
    }

    public static async Task DeleteFileBasicUsage()
    {
        // snippet.delete_file_basic_usage
        PNResult<PNDeleteFileResult> deleteFileResponse = await pubnub.DeleteFile()
            .Channel("my_channel")
            .FileId("d9515cb7-48a7-41a4-9284-f4bf331bc770")
            .FileName("cat_picture.jpg")
            .ExecuteAsync();
        PNDeleteFileResult deleteFileResult = deleteFileResponse.Result;
        PNStatus deleteFileStatus = deleteFileResponse.Status;
        if (!deleteFileStatus.Error && deleteFileResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(deleteFileResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(deleteFileStatus));
        }
        // snippet.end
    }

    public static async Task PublishFileMessageBasicUsage()
    {
        // snippet.publish_file_message_basic_usage
        PNResult<PNPublishFileMessageResult> publishFileMsgResponse = await pubnub.PublishFileMessage()
            .Channel("my_channel")
            .FileId("d9515cb7-48a7-41a4-9284-f4bf331bc770")
            .FileName("cat_picture.jpg") //checks the bin folder if no path is provided
            .Message("This is a sample message")
            .CustomMessageType("file-message")
            .ExecuteAsync();
        PNPublishFileMessageResult publishFileMsgResult = publishFileMsgResponse.Result;
        PNStatus publishFileMsgStatus = publishFileMsgResponse.Status;
        if (!publishFileMsgStatus.Error && publishFileMsgResult != null)
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(publishFileMsgResult));
        }
        else
        {
            Debug.Log(pubnub.JsonPluggableLibrary.SerializeToJsonString(publishFileMsgStatus));
        }
        // snippet.end
    }
}