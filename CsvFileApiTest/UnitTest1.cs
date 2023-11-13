using CsvFileApi.Models;
using Newtonsoft.Json;
using System;
using System.Reflection.Metadata;
using System.Web;
using System.Xml.Linq;

namespace CsvFileApiTest
{
    public class Tests
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseUrl = "https://localhost:5001";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            var url = _baseUrl + "/api/Files";
            var csvFilePath = "./usersCSV.csv";

            if (!System.IO.File.Exists(csvFilePath))
            {
                var rowsCsv = new List<string>
                {
                    "name,city,country,favorite_sport",
                    "John Doe,New York,USA,Basketball",
                    "Jane Smith,London,UK,Football",
                    "Mike Johnson,Paris,France,Tennis",
                    "Karen Lee,Tokyo,Japan,Swimming",
                    "Tom Brown,Sydney,Australia,Running",
                    "Emma Wilson,Berlin,Germany,Basketball"
                };

                Directory.CreateDirectory(Path.GetDirectoryName(csvFilePath));

                using (var streamWriter = new StreamWriter(csvFilePath))
                {
                    foreach (var row in rowsCsv)
                    {
                        streamWriter.WriteLine(row);
                    }
                }
            }

            using (var content = new MultipartFormDataContent())
            using (var fileStream = File.OpenRead(csvFilePath))
            {
                var fieldName = "file";

                content.Add(new StreamContent(fileStream), fieldName, Path.GetFileName(csvFilePath));

                var response = await _httpClient.PostAsync(url, content);

                Assert.That((int)response.StatusCode, Is.EqualTo(200));
            }

            Assert.Pass();
        }

        [Test]
        public async Task Test2()
        {
            var url = _baseUrl + "/api/Users?q=city:paris";
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(1));
            Assert.That(dados[0].city, Is.EqualTo("Paris"));
            Assert.That(dados[0].country, Is.EqualTo("France"));
            Assert.Pass();
        }

        [Test]
        public async Task Test3()
        {
            var url = _baseUrl + "/api/Users?q=city:paris|country:usa";
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(2));
            Assert.That(dados[0].city, Is.EqualTo("New York"));
            Assert.That(dados[0].country, Is.EqualTo("USA"));
            Assert.That(dados[1].city, Is.EqualTo("Paris"));
            Assert.That(dados[1].country, Is.EqualTo("France"));
            Assert.Pass();
        }

        [Test]
        public async Task Test4()
        {
            var url = _baseUrl + "/api/Users?q=name:Tom Brown|country:usa";
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(2));
            Assert.That(dados[0].city, Is.EqualTo("New York"));
            Assert.That(dados[0].country, Is.EqualTo("USA"));
            Assert.That(dados[1].city, Is.EqualTo("Sydney"));
            Assert.That(dados[1].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test5()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:Tom Brown+country:usa");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(0));
            Assert.Pass();
        }

        [Test]
        public async Task Test6()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:*Brown+country:Au*");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(1));
            Assert.That(dados[0].city, Is.EqualTo("Sydney"));
            Assert.That(dados[0].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test7()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:Tom*+country:*lia");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(1));
            Assert.That(dados[0].city, Is.EqualTo("Sydney"));
            Assert.That(dados[0].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test8()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:* *");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(6));
            Assert.That(dados[0].city, Is.EqualTo("New York"));
            Assert.That(dados[0].country, Is.EqualTo("USA"));
            Assert.That(dados[4].city, Is.EqualTo("Sydney"));
            Assert.That(dados[4].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test9()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:T* *B*n");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(1));
            Assert.That(dados[0].city, Is.EqualTo("Sydney"));
            Assert.That(dados[0].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test10()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:T* *B*n|city:t*");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(2));
            Assert.That(dados[0].city, Is.EqualTo("Tokyo"));
            Assert.That(dados[0].country, Is.EqualTo("Japan"));
            Assert.That(dados[1].city, Is.EqualTo("Sydney"));
            Assert.That(dados[1].country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test11()
        {
            var url = _baseUrl + "/api/Users?q=" + Uri.EscapeDataString("name:j*+country:u*+favorite_sport:*ball");
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(2));
            Assert.That(dados[0].name, Is.EqualTo("John Doe"));
            Assert.That(dados[0].city, Is.EqualTo("New York"));
            Assert.That(dados[0].country, Is.EqualTo("USA"));
            Assert.That(dados[0].favorite_sport, Is.EqualTo("Basketball"));
            Assert.That(dados[1].city, Is.EqualTo("London"));
            Assert.That(dados[1].country, Is.EqualTo("UK"));
            Assert.That(dados[1].favorite_sport, Is.EqualTo("Football"));
            Assert.That(dados[1].name, Is.EqualTo("Jane Smith"));
            Assert.Pass();
        }
    }
}