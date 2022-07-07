using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Ridwan.Service.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Util
{
    public interface IProcessingUtil
    {
        Task ProcessDocument(FileUploadRequest request, int inputSize, int outputSize);
    }
    public class ProcessingUtil : IProcessingUtil
    {
        public async Task ProcessDocument(FileUploadRequest request, int inputSize, int outputSize )
        {
            var rows = await ReadContentFromFile(request.FileReference);
            int counter = 0;
            int rowsCount = rows.Count();

            List<OutputModel> result = new List<OutputModel>();
            while (counter < rows.Count())
            {
                List<UploadData> currentSet = new List<UploadData>();

                for (int i = 0; i < inputSize; i++)
                {
                    if (counter < rowsCount)
                    {
                        currentSet.Add(rows.ElementAt(counter));
                        counter++;
                    }
                    
                }

                int currentSetCount = currentSet.Count();
                
                if (currentSetCount > 0)
                {
                    decimal mean = CalculateMean(currentSet, currentSetCount);

                    decimal variance = CalculateVariance(currentSet, currentSetCount, mean);

                    int countCountry = CountUniqueCountry(currentSet, currentSetCount);
                    int countAsn = CountUniqueASN(currentSet, currentSetCount);
                    int countIp = CountUniqueIP(currentSet, currentSetCount);
                    int countDomain = CountUniqueDomain(currentSet, currentSetCount);

                    var output = new OutputModel()
                    {
                        NoOfUniqueCountry = countCountry,
                        NoOfUniqueASN = countAsn,
                        AvgTTL = mean,
                        VarianceTTL = variance,
                        NoOfUniqueIP = countIp,
                        NoOfUniqueDomain = countDomain
                    };

                    for (int k = 0; k < outputSize; k++)
                    {
                        result.Add(output);
                    }
                }

                if (counter != rowsCount)
                    counter--;
            }

            var fileName = RandomString(10);
            ExcelWriter.ListToExcel(result, fileName);
        }

        private int CountUniqueDomain(List<UploadData> currentSet, int currentSetCount)
        {
            Dictionary<string, int> hm = new Dictionary<string, int>();

            for (int i = 0; i < currentSetCount; i++)
            {
                if (!hm.ContainsKey(currentSet.ElementAt(i).Domain))
                {
                    hm.Add(currentSet.ElementAt(i).Domain, 1);
                }
            }

            var countKeys = hm.Count();

            return countKeys;
        }

        private int CountUniqueIP(List<UploadData> currentSet, int currentSetCount)
        {
            Dictionary<string, int> hm = new Dictionary<string, int>();

            for (int i = 0; i < currentSetCount; i++)
            {
                if (!hm.ContainsKey(currentSet.ElementAt(i).IP))
                {
                    hm.Add(currentSet.ElementAt(i).IP, 1);
                }
            }

            var countKeys = hm.Count();

            return countKeys;
        }

        private int CountUniqueASN(List<UploadData> currentSet, int currentSetCount)
        {
            Dictionary<string, int> hm = new Dictionary<string, int>();

            for (int i = 0; i < currentSetCount; i++)
            {
                if (!hm.ContainsKey(currentSet.ElementAt(i).ASN))
                {
                    hm.Add(currentSet.ElementAt(i).ASN, 1);
                }
            }

            var countKeys = hm.Count();

            return countKeys;
        }

        private int CountUniqueCountry(List<UploadData> currentSet, int currentSetCount)
        {
            Dictionary<string, int> hm = new Dictionary<string, int>();

            for (int i = 0; i < currentSetCount; i++)
            {
                if (!hm.ContainsKey(currentSet.ElementAt(i).Country))
                {
                    hm.Add(currentSet.ElementAt(i).Country, 1);
                }
            }

            var countKeys = hm.Count();

            return countKeys;

        }

        private decimal CalculateVariance(List<UploadData> currentSet, int currentSetCount, decimal mean)
        {
            decimal total = 0;

            for (int i = 0; i < currentSetCount; i++)
            {
                var temp = currentSet.ElementAt(i).TTL - mean;
                var square = temp * temp;
                total += square;
            }

            var variance = total / currentSetCount;

            return variance;
        }

        private decimal CalculateMean(List<UploadData> currentSet, int currentSetCount)
        {
            decimal total = 0;

            for (int i = 0; i < currentSetCount; i++)
            {
                total += currentSet.ElementAt(i).TTL;
            }

            var mean = total / currentSetCount;

            return mean;
        }

        private async Task<IEnumerable<UploadData>> ReadContentFromFile(IFormFile file)
        {
            IEnumerable<UploadData> rows = null;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        AllowComments = false,
                        HasHeaderRecord = true,
                        MissingFieldFound = null
                    };

                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    StreamReader reader = new StreamReader(memoryStream);
                    var csv = new CsvReader(reader, conf);
                    rows = csv.GetRecords<UploadData>().ToList();
                    reader.Close();
                    reader.Dispose();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to read offence file: {ex.Message} {ex.StackTrace}");
                throw new AppException("Invalid File. Please try again");
            }

            return rows;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
