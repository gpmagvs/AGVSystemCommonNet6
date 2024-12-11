using AGVSystemCommonNet6.HttpTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Microservices.AudioPlay
{
    public class AudioPlayService
    {
        public static string AudioPlayHostUrl => "http://127.0.0.1:5050";

        public const string GetInformationRoute = "/api/Audio/Information";
        public const string PlayAudioRoute = "/api/Audio/PlayAudio?audioFilePath=";
        public const string StopAudioRoute = "/api/Audio/StopAudio?audioFilePath=";
        public const string AddAudioToQueueRoute = "/api/Audio/AddAudioToQueue?audioFilePath=";
        public const string RemoveAudioFromQueueRoute = "/api/Audio/RemoveAudioFromQueue?audioFilePath=";
        public const string StopAllRoute = "/api/Audio/StopAll";

        public static async Task PlaySpecficAudio(string audioName, double duration = 1)
        {
            try
            {
                HttpHelper http = new HttpHelper(AudioPlayHostUrl);
                (bool success, string json) = await http.PostAsync(PlayAudioRoute + audioName + $"&duration={duration}", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static async Task StopSpecficAudio(string audioName)
        {
            try
            {
                HttpHelper http = new HttpHelper(AudioPlayHostUrl);
                (bool success, string json) = await http.PostAsync(StopAudioRoute + audioName, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static async Task AddAudioToPlayQueue(string audioName)
        {
            try
            {
                HttpHelper http = new HttpHelper(AudioPlayHostUrl);
                (bool success, string json) = await http.PostAsync(AddAudioToQueueRoute + audioName, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static async Task RemoveAudioFromQueue(string audioName)
        {
            try
            {
                HttpHelper http = new HttpHelper(AudioPlayHostUrl);
                (bool success, string json) = await http.PostAsync(RemoveAudioFromQueueRoute + audioName, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static async Task StopAll()
        {
            try
            {
                HttpHelper http = new HttpHelper(AudioPlayHostUrl);
                (bool success, string json) = await http.PostAsync(StopAllRoute, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
