using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ispProject.Models
{
    public class AdminModel
    {

        private string mrn = "";
        private string lName = "Patient Last Not Set";
        private string fName = "Patient First Not Set";
        private string middleI = "";
        private string AliaslName = "";
        private string AliasfName = "";
        private string AliasmiddleI = "";
        private string maidenName = "";
        private string DOB = "";
        private string ssn = "";

        patient p = new patient();
        patient_address pa = new patient_address();
        patient_name pn = new patient_name();


        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();


        /*GET ADMIN OPTIONS*/

        public List<aadmin_data_admission_type> getAdmissionType()
        {

            return db.aadmin_data_admission_type.ToList();
        }

        public List<hospital_doctor> getDoctors()
        {

            return db.hospital_doctor.ToList();
        }

        public List<hospital> getHospitals()
        {

            return db.hospitals.ToList();
        }

        public List<patient_insurance> getPatientInsurance(int id)
        {

            return db.patient_insurance.Where(r => r.patient_id == id).ToList();
        }

        public List<hospital_facility> getFacility()
        {

            return db.hospital_facility.ToList();
        }

        public string getCompanyName(int id)
        {

            aadmin_data_insurance insuranceName = db.aadmin_data_insurance.Where(r => r.Insurance_id == id).FirstOrDefault();
            return insuranceName.company_name;

        }

        public List<aadmin_data_patient_gender> getGenders()
        {

            return db.aadmin_data_patient_gender.ToList();
        }
        public List<aadmin_data_insurance> getInsuranceOptions()
        {
            return db.aadmin_data_insurance.ToList();
        }
        public List<aadmin_data_marital_status> getMaritalStatus()
        {

            return db.aadmin_data_marital_status.ToList();
        }
        public List<aadmin_data_patient_race> getRace()
        {

            return db.aadmin_data_patient_race.ToList();
        }
        public List<aadmin_data_patient_ethnicity> getEthnicity()
        {

            return db.aadmin_data_patient_ethnicity.ToList();
        }
        public patient getPatientInfoByMRN(string mrn)
        {
            this.mrn = mrn;
            return db.patients.Where(r => r.medical_record_number == mrn).FirstOrDefault();
        }
        public patient getPatientInfoBySSN(string ssn)
        {
            patient p = db.patients.Where(r => r.social_security_number == ssn).FirstOrDefault();
            this.ssn = ssn;
            this.mrn = p.medical_record_number;
            return p;
        }
        public void addPatientInformation()
        {

            db.patients.Add(p);
            db.SaveChanges();

        }


        //Get Patients Details Page Items

        public patient getPatientByMRN(string mrn) {

            return db.patients.Where(r => r.medical_record_number == mrn).FirstOrDefault();


        }

        public List<patient_address> getPatientAddressById(int id) {

            List<patient_address> addresses = new List<patient_address>();
            List<patient_address> addressesDB = db.patient_address.Where(r => r.patient_id == id).ToList();
            if (addressesDB.Any()) {
                return addressesDB;
            }
            else {
                patient_address pa = new patient_address();
                pa.city = "waukesha";
                addresses.Add(pa);
                return addresses;
         
            }
        }

        public List<patient_insurance> getInsuranceList(int id) {

            return db.patient_insurance.Where(r => r.patient_id == id).ToList();

        }

        public List<patient_name> getPatientNamesById(int id)
        {

            return db.patient_name.Where(r => r.patient_id == id).ToList();

        }

        public patient_name getPatientPrimaryName(List<patient_name> nameList)
        {
            patient_name primary = null;
            if (nameList.Count == 0) { }
            else {
                foreach (patient_name pn in nameList)
                {
                    if (pn.patient_name_type_id == 1)
                    {
                        primary = pn;
                    }
                }
            }
            return primary;
        }

        public patient_address getPrimaryAddress(List<patient_address> addressList)
        {
            patient_address primary = null;
            foreach (patient_address pn in addressList)
            {
                if (pn.address_type_id == 1)
                {
                    primary = pn;
                }
            }
            return primary;
        }

        public patient_address getBillingAddress(List<patient_address> addressList)
        {
            patient_address primary = null;
            foreach (patient_address pn in addressList)
            {
                if (pn.address_type_id == 2)
                {
                    primary = pn;
                }
            }
            return primary;
        }

        public patient_birth_information getPatientBirthInfoById(int id)
        {

            return db.patient_birth_information.Where(r => r.patient_id == id).FirstOrDefault();

        }

        public List<patient_family> getPatientFamilyById(int id)
        {

            return db.patient_family.Where(r => r.patient_id == id).ToList();

        }

        public int getPatientAge(DateTime bd)
        {
            var today = DateTime.Today;
            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (bd.Year * 100 + bd.Month) * 100 + bd.Day;
            return (a - b) / 10000;

        }


        //End get patient details PAge Items



        public void updatePatient()
        {

            patient uniquePatient = (patient)db.patients.Where(r => r.medical_record_number == this.mrn);

            uniquePatient = p;

            //db.SubmitChanges();

        }

        public string getMRN()
        {   
                return mrn;
        }
        public void setMRN(string mrn)
        {

            this.mrn = mrn;

        }
        public void setPatientLName(string lName)
        {

            this.lName = lName;

        }

        public string getLName()
        {

            return this.lName;

        }
        public void setPatientFName(string fName)
        {

            this.fName = fName;

        }

        public string getFName()
        {

            return this.fName;

        }


        public void setGeneral(FormCollection form)
        {



        }


    }
}