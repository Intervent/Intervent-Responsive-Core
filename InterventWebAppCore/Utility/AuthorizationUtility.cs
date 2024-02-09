using System.Security.Cryptography;
using System.Text;

namespace InterventWebApp
{
    public static class AuthorizationUtility
    {
        private static Dictionary<string, string> KeyList = new Dictionary<string, string>
        {
            {"EHI", "b3a8a011ae0cdde2c5d02bbb4b2e4328af3f2578a060508953810fad8fa9749537f6a1138166db6c966bb74d4b0ab264231f5be3f433bef19b1419f654f7623c"},
            {"EHIProd", "f2ccb128558f88d1998af988d6a4bb026aa9a7f74c024a6f9e0d8ce9eb224228ebff5ead0d4d01a1f75730f84fb5d4b6a1463c9e796b5a52ce5bc58488856b6e"},
            {"Salesforce", "a906cd66f03e3a569e7620337ce9a3924770119bab129be174377dc7ffe63b68abcea03690b436ee8fedfcffc2ee4398933b7a7f0704073696f58f3e97640079" },
            {"SalesforceProd", "419b37008a72804e01a70f0f30b1f69f6cac535f96c29dad501cdbab0149ac973a15980384585332fa9ccbe57e0bac19a55abe35fafbaab124c9bff087abf1c3" },
            {"Intuity", "8bcebfca374887edbc2ac86ae0ebfab72d1e31cc188f1a4d28ac8d05161e01645be1468a2aa9936f859b6e043f1b4b9e64bd6c9f52a7cf838c599ebac1635176" },
            {"IntuityProd", "034fdc5ff165066a1da4aaf19d90b4ea3276dee75af62010ab1b3c84016c9a9f29134b4cee75bb4eaa1f8842211f3f57dbf7e5462f9a8cd0530a3e2d85952a18" },
            {"Intuity2", "b7d2f1e861e7af1e26a4fdfd494a59790e1ef60b6cf7714e66d3dd50c37deb33795a86a60727a8c8c43bb8e2f8941cb83cfcd838ea0a07b32aea61559e5f1188" },
            {"Intuity2Prod", "f769e83e43c5ad67857997db3d0f6a7a12748500edde2f35b8ab75c0b8c198e92cd2b967b1fc9ec829d528c6d15eb0af3c10d5ed4d97801f77d26ced70b61664" },
            {"Placer", "ec4f733de56aede32252910c202ac293a489629d5c9b22493094aed24acd6907e7fc9363659073a757499040440cba194896e6f7a77b6394282ebd667912b4d9" },
            {"PlacerProd", "7580d1d61d8107f8394fb9dee008b387c56e08cbf7055533fb9efa254eb21c6c465dbe22a039c4b3ca1dde9b9e91a3c2420fa5f11cae98f65173c684751f0255" },
        };

        public static bool VerifySecretKey(HttpRequest request, string controller, bool requiresHttps)
        {
            try
            {
                var headers = request.Headers;
                var segments = request.Path.Value.Split('/');
                var client = segments[segments.Length - 2];
                client = client.Replace("/", "");
                var date = request.Query["date"];
                //date should be within a day: Pass ISO date format
                //can change into an hour

                var secretKey = KeyList[client];
                if (requiresHttps)
                    secretKey = KeyList[client + "Prod"];

                if (KeyList.ContainsKey(client) && DateTime.Compare(DateTime.Parse(date), DateTime.UtcNow.AddDays(-1)) >= 0)
                {
                    var user = segments.Last();
                    var keyToSign = string.Format("{0}/{1}/{2}/{3}/{4}", request.Method.ToString().ToUpper(), controller, user, client, date);

                    var authorizationHeader = headers.FirstOrDefault(h => h.Key.Equals("Authorization"));
                    string signedValue = authorizationHeader.Value.FirstOrDefault();

                    using (var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(secretKey)))
                    {
                        var newValue = ByteToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(keyToSign)));
                        return newValue.Equals(signedValue);
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool VerifySecretKey(string authorization, string datetime, string key, bool requiresHttps)
        {
            try
            {
                var keyToSign = string.Format("{0}/{1}", key, datetime);
                var secretKey = KeyList["Intuity"];

                if (requiresHttps)
                    secretKey = KeyList["IntuityProd"];
                if (DateTime.Compare(DateTime.Parse(datetime), DateTime.UtcNow.AddDays(-1)) >= 0)
                {
                    using (var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(secretKey)))
                    {
                        var newValue = ByteToString(hmac.ComputeHash(Encoding.ASCII.GetBytes(keyToSign)));
                        return newValue.Equals(authorization);
                    }
                }
            }
            catch { }
            return false;
        }

        static string ByteToString(IEnumerable<byte> data)
        {
            return string.Concat(data.Select(b => b.ToString("x2")));
        }
    }
}
