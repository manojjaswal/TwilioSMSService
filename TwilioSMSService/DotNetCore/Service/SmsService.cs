using HC.Model;
using HC.Patient.Model.APIKeyConfigurations;
using HC.Patient.Repositories.IRepositories.APIKeyConfigurations;
using HC.Patient.Repositories.IRepositories.Patient;
using HC.Patient.Repositories.IRepositories.User;
using HC.Patient.Service.IServices.Sms;
using HC.Service;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HC.Patient.Service.Services.Sms
{
    public class SmsService: BaseService, ISmsService
    {

   
        private readonly IUserRepository _usersRepository;
        private readonly IAPIKeyConfigurationsRepository _apikeyrepository;
        private readonly IPatientRepository _patientRepository;




        public SmsService( IUserRepository usersRepository, IAPIKeyConfigurationsRepository apikeyrepository, IPatientRepository patientRepository)
        {
            _apikeyrepository = apikeyrepository;
            _usersRepository = usersRepository;
            _patientRepository = patientRepository;
        }


        public string SendSms(MessageModel messageModel)
        {
            try
            {
                string messageSID = string.Empty;
                string TwililoAuthToken ="";
                string TwililoAccoutSid = "";
                string PhoneNumber = "";

                var data = _apikeyrepository.GetAllApiKeys();
                if(data!=null)
                {
                    data.ForEach(key =>
                    {
                        if (key.ConfigType == "Twilio" && key.Key == "AUTH_TOKEN")
                        {
                            TwililoAuthToken = key.value;

                        }
                        else if (key.ConfigType == "Twilio" && key.Key == "ACCOUNT_SID")
                        {
                            TwililoAccoutSid = key.value;

                        }
                        else if (key.ConfigType == "Twilio" && key.Key == "PhoneNumber")
                        {
                            PhoneNumber = key.value;

                        }
                    });

                    messageModel.From = PhoneNumber;
                    messageModel.toNumber = "+91 9816212844";
                    if (!string.IsNullOrEmpty(TwililoAuthToken))
                    {
                        TwilioClient.Init(TwililoAccoutSid, TwililoAuthToken);
                        var SendTo = new PhoneNumber(messageModel.toNumber);
                        var message = MessageResource.Create(SendTo, from: new PhoneNumber(messageModel.From), body: messageModel.Message);
                        messageSID = message.Sid;
                    }

                }

                return messageSID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //Generate and save otp in db.
        public string GenerateSMSPin(int userId)
        {
            #region Declaration
            string pin = string.Empty;
            string smsPin = string.Empty;
           bool isUpdated = false;
            #endregion Declaration

            #region Body
            try
            {
                //Generate SMS pin.
                pin = this.GeneratePIN();
                if (!string.IsNullOrEmpty(pin))
                {
                    // Save OTP in DB.
                    isUpdated = _patientRepository.updatePatientOTP(userId, pin);
                    if (isUpdated)
                    {
                        smsPin = pin;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return smsPin;
            #endregion Body
        }

        /// <summary>
        /// Generate PIN
        /// </summary>
        /// <returns></returns>
        private string GeneratePIN()
        {
            #region Body
            string exactPin = GenerateNewPIN();

            //If PIN contains 0 at starting
            if (exactPin.IndexOf("0") == 0 || exactPin.Trim().StartsWith("0"))
            {
                //generate PIN again
                exactPin = GeneratePIN();
            }
            //Return PIN
            return exactPin;

            #endregion Body
        }
        /// <summary>
        /// Generate New PIN
        /// </summary>
        /// <returns></returns>
        private string GenerateNewPIN()
        {
            #region Declaration          
            var bytes = new byte[4];
            string pin = string.Empty;
            #endregion Declaration

            #region Body            
            //Create new random number
            var rng = RandomNumberGenerator.Create();

            ////Get bytes
            rng.GetBytes(bytes);

            //Convert number 
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000;

            //Get PIN with 5 digit
            string exactPin = String.Format("{0:D5}", random);

            //Return PIN
            return exactPin;

            #endregion Body
        }





    }
}
