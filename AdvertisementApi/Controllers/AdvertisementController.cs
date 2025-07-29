using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AdvertisementApi.Contracts;
using AdvertisementApi.Core;
using AdvertisementApi.Core.Interfaces;

namespace AdvertisementApi.Controllers
{
    [Route("api/adv")]
    [ApiController]
    public sealed class AdvertisementController : ControllerBase
    {
        private readonly long _fileSizeLimit = 100_000;
        private readonly IFileStorage _fileStorage;
        private readonly IAdvertisementModelFactory _advertisementModelFactory;
        private readonly IAdvertisementDataParser _advertisementDataParser;
        private readonly IAdvertisementModelProvider _advertisementModelProvider;

        public AdvertisementController(IOptions<AdvertisementConfig> config, IFileStorage fileStorage,
            IAdvertisementModelFactory factory, IAdvertisementDataParser parser, IAdvertisementModelProvider model)
        {
            _fileSizeLimit = config.Value.fileSizeLimit;
            _fileStorage = fileStorage;
            _advertisementModelFactory = factory;
            _advertisementDataParser = parser;
            _advertisementModelProvider = model;
        }

        [HttpPost("add")]
        public async Task<ActionResult<AdvertisementDataUploadResponse>> AddFile(IFormFile file)
        {
            if (file.Length > _fileSizeLimit)
            {
                return BadRequest($"File size limit reached ({file.Length} > {_fileSizeLimit})");
            }

            string parseMessage;

            using (MemoryStream memoryStream = new((int)file.Length))
            {
                await file.CopyToAsync(memoryStream);

                if (!_advertisementDataParser.TryParse(memoryStream.GetBuffer(), out parseMessage,
                    out List<AdvertisementData> dataList))
                {
                    return BadRequest(parseMessage);
                }

                AdvertisementModel newModel = _advertisementModelFactory.Create(dataList);

                _advertisementModelProvider.SetModel(newModel);

                _fileStorage.SaveFile(memoryStream);
            }

            AdvertisementDataUploadResponse result = new AdvertisementDataUploadResponse(parseMessage);

            return Ok(result);
        }

        [HttpGet("get")]
        public ActionResult<AdvertisementDataResponse> GetList(string[] locations)
        {
            if (!_advertisementModelProvider.initialized)
            {
                if (!_advertisementDataParser.TryParse(_fileStorage.ReadFile(), out string parseMessage,
                    out List<AdvertisementData> dataList))
                {
                    return BadUninitialized();
                }

                AdvertisementModel newModel = _advertisementModelFactory.Create(dataList);

                if (!newModel.IsValid())
                {
                    return BadUninitialized();
                }

                _advertisementModelProvider.SetModel(newModel);
            }

            List<string> result = new List<string>(4);

            foreach (string location in locations)
            {
                result.AddRange(_advertisementModelProvider.FindPlatforms(location));
            }

            return Ok(new AdvertisementDataResponse(result.Distinct().ToArray()));
        }

        private ActionResult<AdvertisementDataResponse> BadUninitialized()
        {
            return BadRequest("Advertisement data is not initialized");
        }
    }
}
