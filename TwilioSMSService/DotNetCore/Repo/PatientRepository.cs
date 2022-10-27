using CreateClinicalReport;
using CreateClinicalReport.Model;
using HC.Common;
using HC.Model;
using HC.Patient.Data;
using HC.Patient.Entity;
using HC.Patient.Model.Patient;
using HC.Patient.Repositories.IRepositories.Patient;
using HC.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using static HC.Common.Enums.CommonEnum;

namespace HC.Patient.Repositories.Repositories.Patient
{
    public class PatientRepository : RepositoryBase<Patients>, IPatientRepository
    {
        private HCOrganizationContext _context;
        private string patientPassword = "password1234";
        public PatientRepository(HCOrganizationContext context) : base(context)
        {
            this._context = context;
        }

        public IQueryable<T> GetPatientDiagnosisCodes<T>(int patientId) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId) };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.GetPatientDiagnosisCodes.ToString(), parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> GetPatientsByTags<T>(string tags, string startWith, int? locationID, bool? isActive) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@tags", tags),
                                          new SqlParameter("@startwith",startWith),
                                          new SqlParameter("@locationId",locationID),
                                          new SqlParameter("@isActive",isActive),
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.GetPatientByTags.ToString(), parameters.Length, parameters).AsQueryable();
        }
        public IQueryable<T> GetPatientGuarantor<T>(int patientId, TokenModel token) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientID", patientId) };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_GetPatientGuarantor.ToString(), parameters.Length, parameters).AsQueryable();
        }

        public PatientInfoDetails GetPatientsDetails(int PatientID, TokenModel token)
        {
            SqlParameter[] parameters = { new SqlParameter("@patientId", PatientID),
            new SqlParameter("@OrganizationId", token.OrganizationID)};
            return _context.ExecStoredProcedureListWithOutputForPatientInfo(SQLObjects.PAT_GetPatientDetails.ToString(), parameters.Length, parameters);
        }

        public PatientHeaderModel GetPatientHeaderInfo(int PatientID, TokenModel token)
        {
            SqlParameter[] parameters = { new SqlParameter("@patientId", PatientID),
            new SqlParameter("@OrganizationId", token.OrganizationID)};
            return _context.ExecStoredProcedureListWithOutputForPatientHeaderInfo(SQLObjects.PAT_GetPatientHeaderInfo.ToString(), parameters.Length, parameters);
        }

        public IQueryable<T> GetActivitiesForPatientPayer<T>(Nullable<int> patientId, string preference, DateTime startDate, DateTime endDate, Nullable<int> patientInsuranceId, TokenModel token) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@Preference", preference),
                                          new SqlParameter("@StartDate", startDate),
                                          new SqlParameter("@EndDate", endDate),
                                          new SqlParameter("@PatientInsuranceId", patientInsuranceId),
                                          new SqlParameter("@OrganizationId", token.OrganizationID)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_GetActivitiesForPatientPayer.ToString(), parameters.Length, parameters).AsQueryable();
        }
        public Dictionary<string, object> GetPatientAuthorizationData(int patientId, int appointmentTypeId, DateTime startDate, string payerPreference)
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@AppointmentTypeId", appointmentTypeId),
                                          new SqlParameter("@Date", startDate),
                                           new SqlParameter("@PayerPreference", payerPreference)
            };
            return _context.ExecStoredProcedureForAuthInfo(SQLObjects.PAT_GetPatientActiveAuthorizationData.ToString(), parameters.Length, parameters);
        }
        public IQueryable<T> GetPatientAddressList<T>(Nullable<int> patientId, int locationId) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@LocationId", locationId)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.APT_GetAddressListOnScheduler, parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> GetPatientPayerServiceCodes<T>(int patientId, string payerPreference, DateTime date, int payerId, int patientInsuranceId) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                            new SqlParameter("@PayerPreference", payerPreference),
                                            new SqlParameter("@Date", date),
                                            new SqlParameter("@PayerId", payerId),
                                            new SqlParameter("@PatientInsuranceId", patientInsuranceId)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_GetPatientPayerServiceCodes, parameters.Length, parameters).AsQueryable();
        }

        public Dictionary<string, object> GetPatientPayerServiceCodesAndModifiers(int patientId, string payerPreference, DateTime? date, int payerId, int patientInsuranceId)
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                            new SqlParameter("@PayerPreference", payerPreference),
                                            new SqlParameter("@Date", date),
                                            new SqlParameter("@PayerId", payerId),
                                            new SqlParameter("@PatientInsuranceId", patientInsuranceId)
            };
            return _context.ExecStoredProcedureForPayerServiceCodesAndModifers(SQLObjects.PAT_GetPatientPayerServiceCodesAndModifiers.ToString(), parameters.Length, parameters);
        }

        public IQueryable<T> GetAuthDataForPatientAppointment<T>(int patientId, int appointmentTypeId, DateTime startDate, DateTime endDate, string payerPreference, Nullable<int> patientAppointmentId, bool isAdmin, Nullable<int> patientInsuranceId, Nullable<int> authorizationId) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@AppointmentTypeId", appointmentTypeId),
                                          new SqlParameter("@FromDate", startDate),
                                          new SqlParameter("@ToDate", endDate),
                                          new SqlParameter("@PayerPreference", payerPreference),
                                          new SqlParameter("@PatientAppointmentId", (patientAppointmentId==0 || patientAppointmentId==null)?null:patientAppointmentId),
                                          new SqlParameter("@IsAdmin", isAdmin),
                                          new SqlParameter("@PatientInsuranceId",patientInsuranceId),
                                          new SqlParameter("@AuthorizationId", authorizationId)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_GetAuthDataForPatientAppointment, parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> CheckServiceCodesAuthorizationForPatient<T>(int patientId, string payerPreference, string serviceCodes, DateTime date) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@PayerPreference", payerPreference),
                                          new SqlParameter("@ServiceCodes", serviceCodes),
                                          new SqlParameter("@Date", date)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_CheckServiceCodesAuthorizationForPatient, parameters.Length, parameters).AsQueryable();
        }

        public bool CheckAuthorizationSetting()
        {
            return true;   ////This may be the setting in database in future.Write the definitions at that time
        }



        public Patients AddPatient(Patients patients)
        {

            _context.Patients.Add(patients);
            _context.SaveChanges();
            return patients;
        }



        /// <summary>
        /// generate patient ccda
        /// </summary>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public MemoryStream GetPatientCCDA(int patientID, TokenModel token)
        {
            PatientInfoDetails patientInfoDetails = GetPatientsDetails(patientID, token);
            PatientClinicalInformation patientClinicalInformation = new PatientClinicalInformation();
            //patient allergies
            MapPatientAllergies(patientInfoDetails, patientClinicalInformation);
            //patient appointment
            MapPatientAppointment(patientInfoDetails, patientClinicalInformation);
            //patient documentation info
            MapDocumentationOfInfo(patientInfoDetails, patientClinicalInformation);
            //patient demographics
            MapPatientDemographics(patientInfoDetails, patientClinicalInformation);
            //patient immunization
            MapPatientImmunization(patientInfoDetails, patientClinicalInformation);
            //patient clinical info
            MapClinicalInfo(patientInfoDetails, patientClinicalInformation);
            // patient patient problems
            MapPatientProblems(patientInfoDetails, patientClinicalInformation);
            // patient social history
            MapPatientSocialHistory(patientInfoDetails, patientClinicalInformation);
            // plan of care
            MapPlanOfCare(patientClinicalInformation);
            // reason for visit
            MapReasonForVisit(patientClinicalInformation);
            //patient lab test
            MapPatientLabTest(patientInfoDetails, patientClinicalInformation);
            // patient encounter
            MapPatientEncounter(patientInfoDetails, patientClinicalInformation);
            // patient funtional status
            MapPatientFuncionalStatus(patientClinicalInformation);
            //patient medication
            MapPatientMedication(patientInfoDetails, patientClinicalInformation);
            // patient vitals
            MapPatientVitals(patientInfoDetails, patientClinicalInformation);
            // patient clinical info 
            MapPatientClinicalInfo(patientInfoDetails, patientClinicalInformation);
            // patient default encounter
            MapDefaultEncounter(patientClinicalInformation);

            ClinicalReportFile clinicalReportFile = new ClinicalReportFile();
            var result = clinicalReportFile.GenerateCCDA(patientClinicalInformation, patientInfoDetails.PatientInfo.FirstOrDefault().Name);

            return result;

        }
        #region Map Patient info for CCDA
        private static void MapDefaultEncounter(PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.EncounterCode = "NA";
                patientClinicalInformation.EncounterDescription = "NA";
                patientClinicalInformation.EncounterDxDate = DateTime.Now.ToShortTimeString();
                patientClinicalInformation.EncounterNoteDate = DateTime.Now.ToShortTimeString();
                patientClinicalInformation.EncounterStaffName = "NA";
                patientClinicalInformation.reasonforTransfer = "NA";
            }
            catch (Exception)
            {
            }
        }

        private void MapPatientClinicalInfo(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptClinicInformation = patientInfoDetails.Organization.Select(p => new ClinicInformation
                {
                    ClinicCity = p.City,
                    ClinicCountry = _context.MasterCountry.Where(k => k.Id == p.CountryID).FirstOrDefault() != null ? _context.MasterCountry.Where(k => k.Id == p.CountryID).FirstOrDefault().CountryName : _context.MasterCountry.FirstOrDefault().CountryName,
                    ClinicName = p.OrganizationName,
                    ClinicPhoneNumber = p.Phone,
                    ClinicStreeet = p.Address1 + p.Address2,
                    ClinicState = _context.MasterState.Where(j => j.Id == p.StateID).FirstOrDefault() != null ? _context.MasterState.Where(j => j.Id == p.StateID).FirstOrDefault().StateName : _context.MasterState.FirstOrDefault().StateName,
                    ClinicZip = p.Zip
                }).FirstOrDefault();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientVitals(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptVitalSigns = patientInfoDetails.PatientVitals.Select(u => new VitalSigns
                {
                    BloodPressure = u.BPSystolic.ToString() + "/" + u.BPDiastolic.ToString(),
                    Entrydate = u.VitalDate,
                    Height = u.HeightIn.Value,
                    VitalsID = new Guid(),
                    WEIGHT = u.WeightLbs

                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientMedication(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptMedication = patientInfoDetails.PatientMedicationModel.Select(g => new CreateClinicalReport.Model.PatientMedication
                {
                    Dose = g.Dose,
                    EndDate = g.EndDate,
                    Frequency = g.Frequency,
                    Medication = g.Medicine,
                    MedicationId = new Guid(),
                    FrequencyID = g.FrequencyID,
                    RxNorm = "NA",
                    StartDate = g.StartDate,
                    Strength = g.Strength,
                    TakingCurrent = true,
                    doseUnit = "NA",
                    Dosage = g.Dose
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientFuncionalStatus(PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptFunctionalStatus = new List<FunctionalStatus>();
                patientClinicalInformation.ptFunctionalStatus.Add(new FunctionalStatus { Code = "NA", Description = "NA", StatusDate = DateTime.Now });
            }
            catch (Exception)
            {
            }
        }

        private void MapPatientEncounter(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptEncounters = new List<Encounters>();
                patientClinicalInformation.ptEncounters = patientInfoDetails.PatientEncounter.Select(d => new Encounters
                {
                    Code = "NA",
                    EncounterDate = d.DateOfService,
                    EncounterDescription = _context.MasterNoteType.Where(h => h.Id == d.NotetypeId).FirstOrDefault() != null ? _context.MasterNoteType.Where(h => h.Id == d.NotetypeId).FirstOrDefault().Type : _context.MasterNoteType.FirstOrDefault().Type,
                    Location = _context.MasterPatientLocation.FirstOrDefault() == null ? "" : _context.MasterPatientLocation.Where(l => l.Id == d.ServiceLocationID).FirstOrDefault() != null ? _context.MasterPatientLocation.Where(l => l.Id == d.ServiceLocationID).FirstOrDefault().Location : _context.MasterPatientLocation.FirstOrDefault().Location,
                    PerformerName = _context.Staffs.Where(o => o.Id == d.StaffID).FirstOrDefault() != null ? _context.Staffs.Where(o => o.Id == d.StaffID).FirstOrDefault().FirstName : _context.Staffs.FirstOrDefault().FirstName,
                }).ToList();
            }
            catch (Exception ex)
            {
            }
        }

        private static void MapPatientLabTest(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptLabResults = patientInfoDetails.PatientLabTestModel.Select(g => new LabResult
                {
                    LonicCode = g.LonicCode,
                    NormalFindings = g.Notes,
                    TestPerformed = g.TestName,
                    TestResultn = g.HL7Result,
                    ReportDate = g.OrderDate,
                    Units = "NA",
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapReasonForVisit(PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptReason = new ReasonForVisit();
                patientClinicalInformation.ptReason.Description = "NA";
                patientClinicalInformation.ptReason.VisitDate = DateTime.Now;
            }
            catch (Exception)
            {
            }
        }

        private static void MapPlanOfCare(PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptPlanOfCare = new List<PlanOfCare>();
                patientClinicalInformation.ptPlanOfCare.Add(new PlanOfCare() { Goal = "NA", Instructions = "NA", PlannedDate = DateTime.Now });
            }
            catch (Exception)
            {
            }
        }

        private void MapPatientSocialHistory(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                var defaultSocialID = _context.GlobalCode.Where(l => l.GlobalCodeCategory.GlobalCodeCategoryName == "socialhistory").FirstOrDefault().Id;
                patientClinicalInformation.ptSocialHistory = patientInfoDetails.PatientSocialHistory.Select(h => new SocialHistoryModel
                {
                    Alcohol = _context.GlobalCode.Where(y => y.Id == h.AlcohalID).FirstOrDefault() != null ? _context.GlobalCode.Where(y => y.Id == h.AlcohalID).FirstOrDefault().GlobalCodeValue : defaultSocialID.ToString(),
                    Drugs = _context.GlobalCode.Where(y => y.Id == h.DrugID).FirstOrDefault() != null ? _context.GlobalCode.Where(y => y.Id == h.DrugID).FirstOrDefault().GlobalCodeValue : defaultSocialID.ToString(),
                    Smoker = "NA",
                    EntryDate = h.CreatedDate != null ? h.CreatedDate.Value.ToShortTimeString() : string.Empty,
                    Tobacoo = _context.GlobalCode.Where(y => y.Id == h.TobaccoID).FirstOrDefault() != null ? _context.GlobalCode.Where(y => y.Id == h.TobaccoID).FirstOrDefault().GlobalCodeValue : defaultSocialID.ToString(),
                }).FirstOrDefault();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientProblems(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptProblemes = patientInfoDetails.PatientDiagnosisDetails.Select(u => new PatientProblemes
                {
                    DateDiagnosed = u.DiagnosisDate.ToShortDateString(),
                    Description = u.Description,
                    PatientGuid = new Guid(),
                    ProblemCode = u.Code,
                    ProblemID = 0,
                    Status = "Active"
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapClinicalInfo(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptProcedure = patientInfoDetails.ClaimServiceLine.Select(o => new ProcedureList
                {
                    CPTCodes = o.ServiceCode,
                    Description = "Description"
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private void MapPatientImmunization(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptImmunization = patientInfoDetails.PatientImmunization.Select(h => new Immunization
                {
                    ApproximateDate = h.AdministeredDate,
                    CVX = 0,
                    Manufacturer = h.ManufacturerName,
                    Vaccine = _context.MasterImmunization.Where(i => i.Id == h.Immunization).FirstOrDefault().VaccineName,
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientDemographics(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptDemographicDetail = patientInfoDetails.PatientInfo.Select(g => new PatientDemographicDetail
                {
                    City = g.City,
                    ClientId = g.PatientID,
                    ContactNo = g.Phone,
                    Country = g.CountryName,
                    DateofBirth = g.DOB.ToShortDateString(),
                    Ethnicity = g.Ethnicity,
                    FirstName = g.FirstName,
                    gender = g.Gender,
                    LanguageCode = "NA",
                    LastName = g.LastName,
                    PreferredLanguage = "NA",
                    Race = g.RaceName,
                    ReasonForReferral = "NA",
                    State = g.StateName,
                    SSN = g.SSN,
                    Street = g.Address,
                    Zip = g.Zip,
                }).FirstOrDefault();
            }
            catch (Exception)
            {
            }
        }

        private static void MapDocumentationOfInfo(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.documentationOfInfo = patientInfoDetails.Staffs.Select(g => new DocumentationOfList
                {
                    address = g.Address,
                    city = g.City,
                    date = g.DOB.ToShortDateString(),
                    pinCode = g.Zip,
                    staffId = g.Id,
                    staffName = g.FirstName + " " + g.LastName,
                    state = "NY",
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientAppointment(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptAppointment = patientInfoDetails.UpcomingAppointmentDetails.Select(j => new FutureAppointment
                {
                    AppointmentDate = j.UpcomingAppointment,
                    DoctorName = j.UpcomingAppointmentStaff
                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private static void MapPatientAllergies(PatientInfoDetails patientInfoDetails, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                patientClinicalInformation.ptAllergies = patientInfoDetails.PatientAllergyModel.Select(k => new CreateClinicalReport.Model.PatientAllergies
                {
                    allergyDate = k.CreatedDate.ToShortDateString(),
                    allergyId = k.PatientAllergyId,
                    reaction = k.Reaction,
                    rxNorm = "NA",
                    status = "Active",
                    substance = k.Allergen,
                    allergen = k.Allergen

                }).ToList();
            }
            catch (Exception)
            {
            }
        }

        #endregion


        /// <summary>
        /// import patient info from ccda
        /// </summary>
        /// <param name="base64File"></param>
        /// <param name="organizationID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int ImportPatientCCDA(string base64File, int organizationID, int userID)
        {
            try
            {
                string filePath = Directory.GetCurrentDirectory() + "\\wwwroot\\CDA\\" + String.Format("{0}_{1:yyyyMMddhhmmss}", "cda", DateTime.Now) + ".xml";
                File.WriteAllBytes(filePath, Convert.FromBase64String(base64File));

                RecordParser recordParser = new RecordParser();
                PatientClinicalInformation patientClinicalInformation = new PatientClinicalInformation();
                patientClinicalInformation = recordParser.ParseCCDAFile(filePath, true);

                //save patient demographics
                Patients patient = SavePatientDemographics(organizationID, userID, patientClinicalInformation);
                //save patient addresses
                SavePatientAddress(userID, patientClinicalInformation, patient);
                //save patient medications
                SavePatientMedications(userID, patientClinicalInformation, patient);
                //save patient allergies
                SavePatientAllergies(userID, patientClinicalInformation, patient);
                //save patient problems
                SavePatientProblems(userID, patientClinicalInformation, patient);
                //save patient social history
                SavePatientSocialHistory(userID, patientClinicalInformation, patient);
                //save patient vitals
                SavePatientVitals(userID, patientClinicalInformation, patient);
                //save patient encounters
                SavePatientEncounters(organizationID, userID, patientClinicalInformation, patient);
                //save patient lab test
                SavePatientLabTest(userID, patientClinicalInformation, patient);
                //save patient immunization
                SavePatientImmunization(userID, patientClinicalInformation, patient);

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetPatientByUserId(int UserId)
        {
            Patients patient = _context.Patients.Where(x => x.UserID == UserId).FirstOrDefault();
            if (patient != null)
                return patient.Id;
            return 0;
        }

        public PatientInfo GetPatientsDetailedInfo(int PatientID, TokenModel token)
        {
            SqlParameter[] parameters = { new SqlParameter("@patientId", PatientID),
            new SqlParameter("@OrganizationId", token.OrganizationID)};
            return _context.ExecStoredProcedureListWithOutputForDetailedPatientInfo(SQLObjects.PAT_GetPatientDetails.ToString(), parameters.Length, parameters);
        }
        #region stripe
        /// <summary>
        /// get user Stripe customerID
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public T GetPatientDetailsForStripe<T>(int patientId) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId) };
            return _context.ExecStoredProcedureWithOutput<T>(SQLObjects.PAT_GetPatientInfoForStripe.ToString(), parameters.Length, parameters);
        }
        public bool UpdateCustomerId(int patientId, string customerId)
        {
            var patient = _context.Patients.Where(z => z.Id == patientId).FirstOrDefault();
            patient.CustomerId = customerId;
            var result = _context.Patients.Update(patient);
            _context.SaveChanges();
            return true;
        }
        #endregion
        #region Save patient info from ccda

        private void SavePatientImmunization(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                List<PatientImmunization> patientImmunization = new List<PatientImmunization>();
                patientImmunization = patientClinicalInformation.ptImmunization.Select(j => new PatientImmunization
                {
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    PatientID = patient.Id,
                    RejectedImmunization = false,
                    ImmunityStatusID = _context.MasterImmunityStatus.FirstOrDefault().Id,//need to change to dynamic
                    ExpireDate = j.ApproximateDate.Value.AddMonths(1),
                    Immunization = _context.MasterImmunization.Where(u => u.VaccineName.Contains(j.Vaccine)).FirstOrDefault() != null ?
                    _context.MasterImmunization.Where(u => u.VaccineName.Contains(j.Vaccine)).FirstOrDefault().Id : _context.MasterImmunization.FirstOrDefault().Id,
                    AdministeredDate = j.ApproximateDate.Value,
                }).ToList();

                _context.PatientImmunization.AddRange(patientImmunization);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
            }
        }

        private void SavePatientLabTest(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
               if(patientClinicalInformation.ptLabResults!=null)
                { 
                List<PatientLabTest> patientLabTest = new List<PatientLabTest>();
                patientLabTest = patientClinicalInformation.ptLabResults.Select(i => new PatientLabTest
                {
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    PatientID = patient.Id,
                    TestName = i.TestPerformed,
                    HL7Result = i.TestResultn,
                    ScheduledDate = Convert.ToDateTime(i.ReportDate),
                    LoincCodeID = _context.MasterLonic.Where(k => k.LonicCode.Contains(i.LonicCode)).FirstOrDefault() != null ?
                    _context.MasterLonic.Where(k => k.LonicCode.Contains(i.LonicCode)).FirstOrDefault().Id : _context.MasterLonic.FirstOrDefault().Id,
                    TestTypeID = 0
                }).ToList();
                _context.PatientLabTest.AddRange(patientLabTest);
                _context.SaveChanges();
            }
            }
            catch (Exception ex)
            {
            }
        }

        private void SavePatientEncounters(int organizationID, int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                if (patientClinicalInformation.ptEncounters != null)
                {
                    List<PatientEncounter> patientEncounters = new List<PatientEncounter>();
                    patientEncounters = patientClinicalInformation.ptEncounters.Select(j => new PatientEncounter
                    {
                        IsActive = true,
                        CreatedBy = userID,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false,
                        DateOfService = Convert.ToDateTime(j.EncounterDate),
                        PatientID = patient.Id,
                        StartDateTime = DateTime.UtcNow,
                        EndDateTime = DateTime.Now.AddMonths(1),
                        StaffID = _context.Staffs.FirstOrDefault().Id,//need to change it to dynamic
                        OrganizationID = organizationID
                    }).ToList();

                    _context.PatientEncounter.AddRange(patientEncounters);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void SavePatientVitals(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                List<PatientVitals> patientVitals = new List<PatientVitals>();
                patientVitals = patientClinicalInformation.ptVitalSigns.Select(l => new PatientVitals
                {
                    //l.Height = ConvertCentimetersToInches(Convert.ToInt32(l.Height)),

                    BMI = calculateBmi(Convert.ToString(string.IsNullOrEmpty(l.HeightUnit) ? "" : l.HeightUnit).ToLower() == "cm" ? ConvertKgToLb(Convert.ToDouble(l.WEIGHT)) : Convert.ToDouble(l.WEIGHT),
                    Convert.ToString(string.IsNullOrEmpty(l.WeightUnit) ? "" : l.WeightUnit).ToLower() == "kg" ? ConvertCentimetersToInches(Convert.ToDouble(l.Height)) : Convert.ToDouble(l.Height)),
                    BPSystolic = Convert.ToInt32(l.BloodPressureSystolic),
                    BPDiastolic = Convert.ToInt32(l.BloodPressureDiastolic),
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    HeartRate = 0,
                    HeightIn = Convert.ToString(string.IsNullOrEmpty(l.HeightUnit) ? "" : l.HeightUnit).ToLower() == "cm" ? ConvertKgToLb(Convert.ToDouble(l.WEIGHT)) : Convert.ToDouble(l.WEIGHT),
                    Pulse = 0,
                    Respiration = 0,
                    Temperature = 0,
                    WeightLbs = Convert.ToString(string.IsNullOrEmpty(l.WeightUnit) ? "" : l.WeightUnit).ToLower() == "kg" ? ConvertCentimetersToInches(Convert.ToDouble(l.Height)) : Convert.ToDouble(l.Height),
                    PatientID = patient.Id,
                    VitalDate = Convert.ToDateTime(l.VitalDate),
                }).ToList();

                _context.PatientVitals.AddRange(patientVitals);
                _context.SaveChanges();

            }
            catch (Exception)
            {
            }
        }

        public double ConvertCentimetersToInches(double centimeters)
        {
            return Math.Round(centimeters / 2.54, 2);
        }

        public static double ConvertKgToLb(double kg)
        {
            return Math.Round(kg * 2.2046226, 2);
        }

        private void SavePatientSocialHistory(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                PatientSocialHistory patientSocialHistory = new PatientSocialHistory();
                patientSocialHistory.IsActive = true;
                patientSocialHistory.CreatedBy = userID;
                patientSocialHistory.CreatedDate = DateTime.UtcNow;
                patientSocialHistory.IsDeleted = false;

                var DefaultSocialStatus = _context.GlobalCode.Where(l => l.GlobalCodeCategory.GlobalCodeCategoryName == "socialhistory" && l.GlobalCodeValue == "Current status unknown").FirstOrDefault().Id;

                patientSocialHistory.AlcohalID = patientClinicalInformation.ptSocialHistory != null && patientClinicalInformation.ptSocialHistory.Alcohol != null ? DefaultSocialStatus : 0;

                //patientSocialHistory.Occupation = "NA";
                patientSocialHistory.DrugID = patientClinicalInformation.ptSocialHistory != null && patientClinicalInformation.ptSocialHistory.Drugs != null ? DefaultSocialStatus : 0;
                patientSocialHistory.TobaccoID = patientClinicalInformation.ptSocialHistory != null && patientClinicalInformation.ptSocialHistory.Tobacoo != null ? DefaultSocialStatus : 0;
                patientSocialHistory.PatientID = patient.Id;
                _context.PatientSocialHistory.Add(patientSocialHistory);
                _context.SaveChanges();

            }
            catch (Exception)
            {
            }
        }

        private void SavePatientProblems(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                List<PatientDiagnosis> patientProblemes = new List<PatientDiagnosis>();
                patientProblemes = patientClinicalInformation.ptProblemes.Select(g => new PatientDiagnosis
                {
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    DiagnosisDate = Convert.ToDateTime(g.DateDiagnosed),
                    ICDID = _context.MasterICD.Where(k => k.Code.Contains(g.Description)).FirstOrDefault() != null ?
                    _context.MasterICD.Where(k => k.Code.Contains(g.Description)).FirstOrDefault().Id : _context.MasterICD.FirstOrDefault().Id,
                    PatientID = patient.Id,
                    ResolveDate = DateTime.Now.AddMonths(1),
                }).ToList();

                _context.PatientDiagnosis.AddRange(patientProblemes);
                _context.SaveChanges();

            }
            catch (Exception)
            {
            }
        }

        private void SavePatientAllergies(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                List<Entity.PatientAllergies> patientAllergies = new List<Entity.PatientAllergies>();

                patientAllergies = patientClinicalInformation.ptAllergies.Select(h => new Entity.PatientAllergies
                {
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    ReactionID = _context.MasterReaction.Where(g => g.Reaction.Contains(h.reaction)).FirstOrDefault() != null ?
                    _context.MasterReaction.Where(g => g.Reaction.Contains(h.reaction)).FirstOrDefault().Id : _context.MasterReaction.FirstOrDefault().Id,
                    Allergen = h.substance,
                    AllergyTypeID = _context.MasterAllergies.FirstOrDefault().Id,
                    PatientID = patient.Id
                }).ToList();

                _context.PatientAllergies.AddRange(patientAllergies);
                _context.SaveChanges();

            }
            catch (Exception)
            {
            }
        }

        private void SavePatientMedications(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                List<Entity.PatientMedication> patientMedication = new List<Entity.PatientMedication>();
                patientMedication = patientClinicalInformation.ptMedication.Select(o => new Entity.PatientMedication
                {
                    IsActive = true,
                    CreatedBy = userID,
                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false,
                    Dose = o.Dose,
                    Medicine = o.Medication,
                    PatientID = patient.Id,
                    StartDate = Convert.ToDateTime(o.StartDate),
                    EndDate = Convert.ToDateTime(o.EndDate),
                    Strength = o.Strength,
                    FrequencyID = o.FrequencyID
                }).ToList();

                _context.PatientMedication.AddRange(patientMedication);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
            }
        }

        private void SavePatientAddress(int userID, PatientClinicalInformation patientClinicalInformation, Patients patient)
        {
            try
            {
                PHIEncryptedModel pHIEncryptedModel = GetEncryptedPHIData<PHIEncryptedModel>(null, null, null, null, null, null, null, null, patientClinicalInformation.ptDemographicDetail.Street, null, patientClinicalInformation.ptDemographicDetail.City, patientClinicalInformation.ptDemographicDetail.Zip, null, null).FirstOrDefault();
                PatientAddress patientAddress = new PatientAddress();
                patientAddress.IsActive = true;
                patientAddress.IsDeleted = false;
                patientAddress.IsMailingSame = true;
                patientAddress.IsPrimary = true;
                patientAddress.PatientID = patient != null ? patient.Id : 0;
                patientAddress.PatientLocationID = patient != null ? patient.LocationID : 0;
                patientAddress.Address1 = pHIEncryptedModel.Address1; //patientClinicalInformation.ptDemographicDetail.Street;
                patientAddress.City = pHIEncryptedModel.City;//patientClinicalInformation.ptDemographicDetail.City;
                patientAddress.Zip = pHIEncryptedModel.ZipCode;//patientClinicalInformation.ptDemographicDetail.Zip;
                var address = _context.GlobalCode.Where(k => k.GlobalCodeCategoryID == _context.GlobalCodeCategory.Where(f => f.GlobalCodeCategoryName == "addresstype").FirstOrDefault().Id && k.GlobalCodeValue == "Primary Home").FirstOrDefault();
                patientAddress.AddressTypeID = address != null ? address.Id : (int?)null;
                //patientAddress.Phone = patientClinicalInformation.ptDemographicDetail.ContactNo.Replace("tel:", "");
                if (patientClinicalInformation.ptDemographicDetail.ContactNo != null)
                {
                    patientAddress.Phone = patientClinicalInformation.ptDemographicDetail.ContactNo.Replace("tel:", "");
                }
                patientAddress.StateID = _context.MasterState.Where(p => p.StateName.Contains(patientClinicalInformation.ptDemographicDetail.State)).FirstOrDefault() != null ?
                    _context.MasterState.Where(p => p.StateName.Contains(patientClinicalInformation.ptDemographicDetail.State)).FirstOrDefault().Id : _context.MasterState.FirstOrDefault().Id;
                patientAddress.CountryID = _context.MasterCountry.Where(l => l.CountryName.Contains(patientClinicalInformation.ptDemographicDetail.Country)).FirstOrDefault() != null ?
                    _context.MasterCountry.Where(l => l.CountryName.Contains(patientClinicalInformation.ptDemographicDetail.Country)).FirstOrDefault().Id : _context.MasterCountry.FirstOrDefault().Id;
                patientAddress.CreatedBy = userID;
                patientAddress.CreatedDate = DateTime.UtcNow;

                _context.PatientAddress.Add(patientAddress);
                _context.SaveChanges();

            }
            catch (Exception)
            {
            }
        }

        private Patients SavePatientDemographics(int organizationID, int userID, PatientClinicalInformation patientClinicalInformation)
        {
            try
            {
                DateTime DOB = new DateTime();
                DateTime.TryParse(patientClinicalInformation.ptDemographicDetail.DateofBirth, out DOB);
                //PHIEncryptedModel pHIEncryptedModel = GetEncryptedPHIData<PHIEncryptedModel>(patientClinicalInformation.ptDemographicDetail.FirstName,null, patientClinicalInformation.ptDemographicDetail.LastName, null, null, patientClinicalInformation.ptDemographicDetail.SSN, String.Format("{0}{1:yyyyMMddhhmmss}", "MRN-", DateTime.Now), null, null, null, null, DOB.ToShortDateString(), null, null).FirstOrDefault();
                Entity.User user = SaveUserInfo(organizationID, userID);
                PHIEncryptedModel pHIEncryptedModel = GetEncryptedPHIData<PHIEncryptedModel>(patientClinicalInformation.ptDemographicDetail.FirstName, null, patientClinicalInformation.ptDemographicDetail.LastName, DOB.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), null, patientClinicalInformation.ptDemographicDetail.SSN, "MRN-", null, null, null, null, null, null, null).FirstOrDefault();
                Patients patient = new Patients();
                patient.FirstName = pHIEncryptedModel.FirstName;// + String.Format("{0:yyyy_MM_dd_hh_mm_ss}", DateTime.Now);
                patient.LastName = pHIEncryptedModel.LastName;// + String.Format("{0:yyyy_MM_dd_hh_mm_ss}", DateTime.Now);
                //DateTime DOB = new DateTime();
                //DateTime.TryParse(patientClinicalInformation.ptDemographicDetail.DateofBirth, out DOB);
                patient.DOB = pHIEncryptedModel.DateOfBirth;
                patient.Email = pHIEncryptedModel.EmailAddress;
                var race =
                patient.Race = _context.MasterRace.Where(k => k.RaceName.Contains(patientClinicalInformation.ptDemographicDetail.Race)).FirstOrDefault() != null ?
                    _context.MasterRace.Where(k => k.RaceName.Contains(patientClinicalInformation.ptDemographicDetail.Race)).FirstOrDefault().Id : _context.MasterRace.FirstOrDefault() == null ? (int?)null : _context.MasterRace.FirstOrDefault().Id;
                //patient.ReferralReason = patientClinicalInformation.ptDemographicDetail.ReasonForReferral;
                patient.SSN = pHIEncryptedModel.SSN;
                //patient.Title = "Mr.";
                patient.MRN = pHIEncryptedModel.MRN;
                patient.Gender = _context.MasterGender.Where(l => l.Gender.Contains(patientClinicalInformation.ptDemographicDetail.gender)).FirstOrDefault() != null ?
                    _context.MasterGender.Where(l => l.Gender.Contains(patientClinicalInformation.ptDemographicDetail.gender)).FirstOrDefault().Id : _context.MasterGender.FirstOrDefault() == null ? 0 : _context.MasterGender.FirstOrDefault().Id;
                patient.IsActive = true;
                patient.OrganizationID = organizationID;//need to get organization id from token
                patient.CreatedBy = userID;//need to get current user from token
                patient.CreatedDate = DateTime.UtcNow;
                patient.IsDeleted = false;
                patient.UserID = user.Id;
                patient.IsPortalActivate = true;
                patient.LocationID = _context.Location.Where(j => j.OrganizationID == organizationID).FirstOrDefault().Id;
                _context.Patients.Add(patient);
                _context.SaveChanges();
                //return null;
                return patient;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private Entity.User SaveUserInfo(int organizationID, int userID)
        {
            try
            {
                var Password = patientPassword; //change it to dynamic for patients
                if (!string.IsNullOrEmpty(Password)) { Password = CommonMethods.Encrypt(Password); }
                int patientRoleID = _context.UserRoles.Where(m => (m.RoleName.ToLower() == "client" || m.RoleName.ToLower() == "patient") && m.OrganizationID == organizationID && m.IsActive == true && m.IsDeleted == false).FirstOrDefault().Id;

                Entity.User user = new Entity.User();
                user.IsActive = true;
                user.OrganizationID = organizationID;//need to get organization id from token
                user.CreatedBy = userID;//need to get current user from token
                user.CreatedDate = DateTime.UtcNow;
                user.IsDeleted = false;
                user.AccessFailedCount = 0;
                user.IsBlock = false;
                user.Password = Password;
                user.RoleID = patientRoleID;
                user.UserName = String.Format("{0}{1:yyyyMMddhhmmss}", "email", DateTime.Now) + "@mailinator.com";
                _context.User.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public IQueryable<T> GetPatients<T>(ListingFiltterModel patientFiltterModel, TokenModel token) where T : class, new()
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@SeachKey", patientFiltterModel.SearchKey),
                                          new SqlParameter("@StartWith",patientFiltterModel.StartWith),
                                          new SqlParameter("@Tags",patientFiltterModel.Tags),
                                          new SqlParameter("@LocationIDs",patientFiltterModel.LocationIDs),
                                          new SqlParameter("@IsActive",patientFiltterModel.IsActive),
                                          new SqlParameter("@OrganizationID",token.OrganizationID),
                                          new SqlParameter("@SortColumn",patientFiltterModel.sortColumn),
                                          new SqlParameter("@SortOrder",patientFiltterModel.sortOrder),
                                          new SqlParameter("@PageNumber",patientFiltterModel.pageNumber),
                                          new SqlParameter("@PageSize",patientFiltterModel.pageSize),
                };
                return _context.ExecStoredProcedureListWithOutput<T>("GetFilteredPatients", parameters.Length, parameters).AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable<T> GetStaffPatients<T>(ListingFiltterModel patientFiltterModel, TokenModel token) where T : class, new()
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@SeachKey", patientFiltterModel.SearchKey),
                                          new SqlParameter("@StartWith",patientFiltterModel.StartWith),
                                          new SqlParameter("@staffid",token.StaffID),
                                          new SqlParameter("@Tags",patientFiltterModel.Tags),
                                          new SqlParameter("@LocationIDs",patientFiltterModel.LocationIDs),
                                          new SqlParameter("@IsActive",patientFiltterModel.IsActive),
                                          new SqlParameter("@OrganizationID",token.OrganizationID),
                                          new SqlParameter("@SortColumn",patientFiltterModel.sortColumn),
                                          new SqlParameter("@SortOrder",patientFiltterModel.sortOrder),
                                          new SqlParameter("@PageNumber",patientFiltterModel.pageNumber),
                                          new SqlParameter("@PageSize",patientFiltterModel.pageSize),
                };
                return _context.ExecStoredProcedureListWithOutput<T>("GetFilteredPatients_v1", parameters.Length, parameters).AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
        }



        public IQueryable<T> GetPatientsForCouponCods<T>(ListingFiltterModel patientFiltterModel, TokenModel token) where T:class,new()
        {
            try
            {

                SqlParameter[] parameters = { new SqlParameter("@SearchText", patientFiltterModel.SearchKey),
                                          new SqlParameter("@OrganizationID",token.OrganizationID),
                                          new SqlParameter("@SortColumn",patientFiltterModel.sortColumn),
                                          new SqlParameter("@SortOrder",patientFiltterModel.sortOrder),
                                          new SqlParameter("@PageNumber",patientFiltterModel.pageNumber),
                                          new SqlParameter("@PageSize",patientFiltterModel.pageSize),
                };
                return _context.ExecStoredProcedureListWithOutput<T>("COUPONCODES_GetAllClients", parameters.Length, parameters).AsQueryable();

            }
            catch(Exception)
            {
                throw;
            }
        }


        public IQueryable<T> GetPatientsByUserName<T>(string UserName) where T :class, new()
        {
            try
            {
                SqlParameter[] parameters = { new SqlParameter("@UserName", UserName) };

                return _context.ExecStoredProcedureListWithOutput<T>("GetPatientByUserName", parameters.Length, parameters).AsQueryable();
            }
            catch(Exception)
            {
                throw;
            }
        }


        public bool updatePatientOTP(int UserId,string OTP)
        {
            try
            {
                var userData = _context.User.Where(c => c.Id == UserId).FirstOrDefault();
                userData.OTP = OTP;
                _context.SaveChanges();
                return true;


            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Helping Methods
        /// <summary>
        /// Calculate BMI and update relevant fields
        /// </summary>
        /// <param name="patientVitals"></param>
        /// <returns></returns>
        private static double calculateBmi(double WeightLbs, double HeightIn)
        {
            double? weightKg = 0;
            double? heightCm = 0;
            double BMI = 0;
            if (WeightLbs > 0)
            {
                //convert lbs into pound (.45 is 1kg value in pounds)
                weightKg = Math.Round(WeightLbs * .45, 2);
            }

            if (HeightIn > 0)
            {
                ////convert height of feet and inches into cm
                //heightCm = Math.Round((double)((patientVitals.HeightFt * 12) + patientVitals.HeightIn) * 2.54, 2);
                //convert height of inches into cm
                heightCm = Math.Round(HeightIn * 2.54, 2);
            }

            //var height = patientVitals.Height_cm;
            //var weight = patientVitals.Weight_kg;

            if (heightCm > 0 && weightKg > 0)
            {
                //calculate BMI
                BMI = Math.Round((double)(weightKg / (heightCm / 100 * heightCm / 100)), 2);

                //if (patientVitals.BMI < 18.5)
                //{
                //    patientVitals.BMI_Status = "Below Normal";
                //}
                //if (patientVitals.BMI > 18.5 && patientVitals.BMI < 25)
                //{
                //    patientVitals.BMI_Status = "Normal";
                //}
                //if (patientVitals.BMI > 25)
                //{
                //    patientVitals.BMI_Status = "Overweight";
                //}
            }
            return BMI;
        }

        public IQueryable<T> GetAuthorizationsForPatientPayer<T>(int patientId, int patientInsuranceId, DateTime startDate, TokenModel token) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@PatientInsuranceId", patientInsuranceId),
                                          new SqlParameter("@StartDate", startDate)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_GetAuthorizationsForPatientPayer, parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> GetEncryptedPHIData<T>(string firstName, string middleName, string lastName, string dob, string emailAddress, string ssn, string mrn, string aptnumber, string address1, string address2, string city, string zipCode, string phonenumber, string healthPlanBeneficiaryNumber) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@FirstName", firstName),
                                          new SqlParameter("@MiddleName", middleName),
                                          new SqlParameter("@LastName", lastName),
                                          new SqlParameter("@DateOfBirth", dob),
                                          new SqlParameter("@EmailAddress", emailAddress),
                                          new SqlParameter("@SSN", ssn),
                                          new SqlParameter("@MRN", mrn),
                                          new SqlParameter("@AptNumber", aptnumber),
                                          new SqlParameter("@Address1", address1),
                                          new SqlParameter("@Address2", address2),
                                          new SqlParameter("@City", city),
                                          new SqlParameter("@ZipCode", zipCode),
                                          new SqlParameter("@Phonenumber", phonenumber),
                                          new SqlParameter("@HealthPlanBeneficiaryNumber", healthPlanBeneficiaryNumber)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.MTR_EncryptPHIData, parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> GetDecryptedPHIData<T>(byte[] firstName, byte[] middleName, byte[] lastName, byte[] dob, byte[] emailAddress, byte[] ssn, byte[] mrn, byte[] aptnumber, byte[] address1, byte[] address2, byte[] city, byte[] zipCode, byte[] phonenumber, byte[] healthPlanBeneficiaryNumber) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@FirstName", firstName),
                                          new SqlParameter("@MiddleName", middleName),
                                          new SqlParameter("@LastName", lastName),
                                          new SqlParameter("@DateOfBirth", dob),
                                          new SqlParameter("@EmailAddress", emailAddress),
                                          new SqlParameter("@SSN", ssn),
                                          new SqlParameter("@MRN", mrn),
                                          new SqlParameter("@AptNumber", aptnumber),
                                          new SqlParameter("@Address1", address1),
                                          new SqlParameter("@Address2", address2),
                                          new SqlParameter("@City", city),
                                          new SqlParameter("@ZipCode", zipCode),
                                          new SqlParameter("@Phonenumber", phonenumber),
                                          new SqlParameter("@HealthPlanBeneficiaryNumber", healthPlanBeneficiaryNumber)
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.MTR_DecryptPHIData, parameters.Length, parameters).AsQueryable();
        }

        public IQueryable<T> CheckExistingPatient<T>(string email, string mrn, string userName, int patientId, TokenModel token) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@Email",email),
                                          new SqlParameter("@MRN", mrn),
                                          new SqlParameter("@UserName", userName),
                                          new SqlParameter("@PatientId", patientId),
                                          new SqlParameter("@OrganizationId",token.OrganizationID )
            };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.PAT_CheckExistingPatient, parameters.Length, parameters).AsQueryable(); //throw new NotImplementedException();
        }
        public IQueryable<T> EncryptMultipleValues<T>(string values) where T : class, new()
        {
            SqlParameter[] parameters = { new SqlParameter("@Data", values.ToString()) };
            return _context.ExecStoredProcedureListWithOutput<T>(SQLObjects.MTR_EncryptMultipleValues.ToString(), parameters.Length, parameters).AsQueryable();
        }
        #endregion
    }
}