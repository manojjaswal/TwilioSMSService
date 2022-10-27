using HC.Model;
using HC.Patient.Model.Patient;
using HC.Patient.Service.IServices.Patient;
using HC.Patient.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HC.Patient.Web.Controllers
{
    [Produces("application/json")]
    [Route("Patients")]
    [ActionFilter]
    public class PatientsController : BaseController
    {
        private readonly IPatientService _patientService;
        private static HttpClient Client { get; } = new HttpClient();

        #region Construtor of the class
        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// <Description> this method is used to create or update the patient entity  </Description>        
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdatePatient")]
        public JsonResult CreateUpdatePatient([FromBody]PatientDemographicsModel patient)
        {
            return Json(_patientService.ExecuteFunctions<JsonModel>(() => _patientService.CreateUpdatePatient(patient, GetToken(HttpContext))));
        }
        /// <summary>
        /// <Description> this method is used to create or update the patient entity  </Description>        
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        [HttpPost("CreateUpdateClient")]
        public JsonResult CreateUpdateClient([FromBody]PatientDemographicsModel patient)
        {
            return Json(_patientService.ExecuteFunctions<JsonModel>(() => _patientService.CreateUpdateClient(patient, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to get patient by id </Description>        
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet("GetPatientById")]
        public JsonResult GetPatientById(int patientId)
        {
            return Json(_patientService.ExecuteFunctions<JsonModel>(() => _patientService.GetPatientById(patientId, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to get patient guarantor  </Description>        
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet("GetPatientGuarantor")]
        public JsonResult GetPatientGuarantor(int patientId)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientGuarantor(patientId, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to get patient listing with filters  </Description>        
        /// </summary>
        /// <param name="patientFiltterModel"></param>
        /// <returns></returns>
        [HttpGet("GetPatients")]
        public JsonResult GetPatients(ListingFiltterModel patientFiltterModel)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatients(patientFiltterModel, GetToken(HttpContext))));
        }




        /// <summary>
        /// <Description> this method is used to get patient listing of Coupon Codes  </Description>        
        /// </summary>
        /// <param name="patientFiltterModel"></param>
        /// <returns></returns>
        [HttpGet("GetPatientsListForCouponCodes")]
        public JsonResult GetPatientsListForCouponCodes(ListingFiltterModel patientFiltterModel)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientsForCouponCods(patientFiltterModel, GetToken(HttpContext))));
        }



        /// <summary>
        /// Description - This action will get all the service codes assigned to a patient payer with their authorization if they are attcahed to any authorization
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPatientPayerServiceCodes")]
        public JsonResult GetPatientPayerServiceCodes(PatientPayerServiceCodeFilterModel patientPayerServiceCodeFilterModel)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientPayerServiceCodes(patientPayerServiceCodeFilterModel.PatientId, patientPayerServiceCodeFilterModel.PayerPreference, Convert.ToDateTime((patientPayerServiceCodeFilterModel.Date == null ? DateTime.UtcNow : patientPayerServiceCodeFilterModel.Date)), patientPayerServiceCodeFilterModel.PayerId, patientPayerServiceCodeFilterModel.PatientInsuranceId, GetToken(HttpContext))));
        }

        /// <summary>
        /// Description - This action will get all the service codes assigned to a patient payer with their authorization if they are attcahed to any authorization
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPatientPayerServiceCodesAndModifiers")]
        public JsonResult GetPatientPayerServiceCodesAndModifiers(PatientPayerServiceCodeFilterModel patientPayerServiceCodeFilterModel)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientPayerServiceCodesAndModifiers(patientPayerServiceCodeFilterModel.PatientId, patientPayerServiceCodeFilterModel.PayerPreference, Convert.ToDateTime((patientPayerServiceCodeFilterModel.Date == null ? DateTime.UtcNow : patientPayerServiceCodeFilterModel.Date)), patientPayerServiceCodeFilterModel.PayerId, patientPayerServiceCodeFilterModel.PatientInsuranceId, GetToken(HttpContext))));
        }

        /// <summary>        
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPatientCCDA")]
        public IActionResult GetPatientCCDA(int id)
        {
            MemoryStream tempStream = null;
            tempStream = _patientService.GetPatientCCDA(id, GetToken(HttpContext));
            if (ReferenceEquals(tempStream, null))
                tempStream = new MemoryStream();

            string fileName = "CDA.zip";

            var fileStreams = new Dictionary<string, string>
            {
                { "CDA.xml", "http://" + Request.Host.Value + "/CDA/CDA.xml" },
                { "CDA.xsl",  "http://"+ Request.Host.Value + "/CDA/CDA.xsl" },
            };
            return new FileCallbackResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
                {
                    foreach (var kvp in fileStreams)
                    {
                        var zipEntry = zipArchive.CreateEntry(kvp.Key);
                        using (var zipStream = zipEntry.Open())
                        using (var stream = await Client.GetStreamAsync(kvp.Value))
                            await stream.CopyToAsync(zipStream);
                    }
                }

                string path = Directory.GetCurrentDirectory() + "\\wwwroot\\CDA\\CDA.xml";
                FileInfo file = new FileInfo(path);
                if (file.Exists)//check file exsit or not
                {
                    file.Delete();
                }
            })
            {
                FileDownloadName = fileName
            };
        }

        /// <summary>
        /// <Description> this method is used to get full detail of patient</Description>   
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPatientsDetails")]
        public JsonResult GetPatientsDetails(int id)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientsDetails(id, GetToken(HttpContext))));
        }
        [HttpGet]
        [Route("GetPatientsDetailsForMobile")]
        public JsonResult GetPatientsDetailsForMobile(int id)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientsDetailsForMobile(id, GetToken(HttpContext))));
        }
        /// <summary>
        /// this method will get the patient basic info for header menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPatientHeaderInfo")]
        public JsonResult GetPatientHeaderInfo(int id)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientHeaderInfo(id, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to get patient by tags  </Description>   
        /// </summary>
        /// <param name="patientFiltterModel"></param>
        /// <returns></returns>
        [HttpGet("GetPatientByTag")]
        public JsonResult GetPatientByTag(ListingFiltterModel patientFiltterModel)
        {   
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientByTags(patientFiltterModel, GetToken(HttpContext))));
        }

        /// <summary>
        /// Description - Get All authorizations for patients
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAuthorizationsForPatient")]
        public JsonResult GetAllAuthorizationsForPatient(int patientId, int pageNumber, int pageSize, string authType)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetAllAuthorizationsForPatient(patientId, pageNumber, pageSize, authType, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to update patient's potal (active/inactive)  </Description>   
        /// </summary>
        /// <returns></returns>
        [HttpPatch("UpdatePatientPortalVisibility")]
        public JsonResult UpdatePatientPortalVisibility(int patientID, int userID, bool isPortalActive)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.UpdatePatientPortalVisibility(patientID, userID, isPortalActive, GetToken(HttpContext))));
        }

        /// <summary>
        /// <Description> this method is used to update patient's status (active/inactive)  </Description>   
        /// </summary>
        /// <returns></returns>
        [HttpPatch("UpdatePatientActiveStatus")]
        public JsonResult UpdatePatientActiveStatus(int patientID, bool isActive)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.UpdatePatientActiveStatus(patientID, isActive, GetToken(HttpContext))));
        }

        [HttpPost]
        [Route("ImportPatientCCDA")]
        public JsonResult ImportPatientCCDA([FromBody]JObject file)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.ImportPatientCCDA(file, GetToken(HttpContext))));
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAuthorizationsForPatientPayer")]
        public JsonResult GetPatientPayerActivities(int patientId, int patientInsuranceId, DateTime startDate)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetAuthorizationsForPatientPayer(patientId, patientInsuranceId, startDate, GetToken(HttpContext))));
        }


        /// <summary>
        /// <Description> this method is used to get patient by tags  </Description>   
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpGet("GetPatientByUserName")]
        public JsonResult GetPatientByUserName(string UserName)
        {
            return Json(_patientService.ExecuteFunctions(() => _patientService.GetPatientsByUserName(UserName)));
        }


        #endregion

        #region Helping Methods
        /////////////////////////
        //helping methods
        #endregion
    }
}