using Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTesting
{
    public class ProductsApiTest
    {
        [Fact]
        public async Task TestGetAll()
        {
            using(var client =new TestClientProvider().Client)
            {
                var respone = await client.GetAsync("/api/Products");
                respone.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
            }
        }

        [Fact]
        public async Task TestPost()
        {
            using (var client = new TestClientProvider().Client)
            {
                var respone = await client.PostAsync("/api/Products/", new StringContent(
                    JsonConvert.SerializeObject(new Product()
                    {
                        Name = "X-Ray Machine",
                        Price = 1000000000,
                        CreateDate = DateTime.Now,
                        Description = "Alat untuk Melihat bagian dalam tubuh"
                    }),
                    Encoding.UTF8, "application/json"
                    )
                );
                respone.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.OK, respone.StatusCode);
            }
        }
    }
}
