using CsvFileApi.Models;
using Newtonsoft.Json;

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
            Assert.That(dados[0].City, Is.EqualTo("Paris"));
            Assert.That(dados[0].Country, Is.EqualTo("France"));
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
            Assert.That(dados[0].City, Is.EqualTo("New York"));
            Assert.That(dados[0].Country, Is.EqualTo("USA"));
            Assert.That(dados[1].City, Is.EqualTo("Paris"));
            Assert.That(dados[1].Country, Is.EqualTo("France"));
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
            Assert.That(dados[0].City, Is.EqualTo("New York"));
            Assert.That(dados[0].Country, Is.EqualTo("USA"));
            Assert.That(dados[1].City, Is.EqualTo("Sydney"));
            Assert.That(dados[1].Country, Is.EqualTo("Australia"));
            Assert.Pass();
        }

        [Test]
        public async Task Test5()
        {
            var url = _baseUrl + "/api/Users?q=name:Tom Brown+country:usa";
            var response = await _httpClient.GetAsync(url);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var dados = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            Assert.That((int)response.StatusCode, Is.EqualTo(200));
            Assert.That(dados.Count, Is.EqualTo(0));
            Assert.Pass();
        }
    }
}