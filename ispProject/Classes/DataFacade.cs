using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ispProject.Models;

namespace ispProject.Classes
{
    public class DataFacade
        : IDisposable
    {
        private HITProjectData_Fall17Entities1 db = new HITProjectData_Fall17Entities1();


        public void Dispose()
        {
            if (db != null)
            {
                db.Database.Connection.Close();
                db = null;
            }
        }

      
   
        public string GetNursingTempRouteName(int? id)
        {

            if (id.HasValue)
            {
                try
                {
                    //use LINQ statement
                    var temp = (from s in this.db.nursing_temp_route_type
                                where s.temp_route_type_id == id.Value
                                select s.temp_route_type_name).First();

                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }

        }
        public string GetNursingPulseRouteName(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_pulse_route_type
                                where s.pulse_route_type_id == id.Value
                                select s.pulse_route_type_name).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }

        }
        public string GetO2DeliveryMethod(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_o_two_delivery_type
                                where s.o_two_delivery_type_id == id.Value
                                select s.o_two_delivery_type_name).First();
                    return temp as string;

                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetPainScaleType(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_pain_scale_type
                                where s.pain_scale_type_id == id.Value
                                select s.pain_scale_type_name).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetPatientName(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.patient_name
                                where s.patient_id == id.Value
                                select s.first_name + s.middle_name + s.last_name).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetPatientBirthdate(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.patient_birth_information
                                where s.patient_id == id.Value
                                select s.birth_date.ToString()).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetPatientMRNNumber(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.patients
                                where s.patient_id == id.Value
                                select s.medical_record_number).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetVitalComment(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_pca_comment
                                where s.pca_id == id.Value
                                select s.pca_comment).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetWdlInfo(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_care_system_assessment
                                where s.pca_id == id.Value
                                select s.wdl_ex.ToString()).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }
        public string GetPainComment(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var temp = (from s in this.db.nursing_care_system_assessment
                                where s.pca_id == id.Value
                                select s.care_system_comment).First();
                    return temp as string;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            else
            {
                return "[No Data]";
            }
        }

    }
}