// HT Enterprises

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;


/// <summary>
/// Summary description for zCustomsInfo
/// </summary>
public class zCustomsInfo
{
    #region Variables
    public string sShipToCompany = "", XLExpressPackageType = "", sShipToContact = "", ShipViaName = "", sShipToAddressLine1 = "", sShipToAddressLine3 = "", sShipToAddressLine2 = "", sShipToCity = "", sShipToState = "", sShipToCountry = "", sShipToZipCode = "", sShipToPhone = "", sShipToEmail = "", sShipToWeight = "", sShipToThirdPartyCarrier = "", sShipToThirdPartyZipCode = "", sShipToThirdPartyCountry = "", sUSRcheck = "", SunsetUsercheck = "False", SunsetuserDescp = "", strSunsetAddress3_block = "", strSunsetAddress3_Buildings = "", sCarrierMethod = "", sCarrierService = "", sCarrierPaymentMethod = "", sPONo = "", sInvoiceNo = "", sThirdpartyaccountno = "", sThirdpartypayment = "", strCODFlag = "", strSatFlag = "";
    public string sPlantCompany = "", sPlantContact = "", sPlantAddressLine1 = "", sPlantAddressLine2 = "", sPlantCity = "", sPlantState = "", sPlantZipCode = "", sPlantCountry = "", sPlantPhone = "", sPlantEmail = "", swhscode = "";
    public string constr;
    public string BillToCode = "";
    public string PlantID = "";
    public string CustomerID = "";
    public string plantres = "";
    public string strAccountNumber = "";
    public string CanadaPostPayorAccountNumber = "";
    public string[] strDelNo;
    SqlConnection SqlCon;
    public string ERPCon = "";
    public string strDelivery = "";
    DataSet DelInfo = new DataSet();
    #endregion
    public zCustomsInfo()
    {
        constr = ConfigurationManager.ConnectionStrings["ECSConnectionString"].ConnectionString;
        SqlCon = new SqlConnection(constr);
    }
    public String addquery(string query)
    {
        try
        {
            String strpath = "";
            strpath = "{0}\\LogFile\\1.txt";
            strpath = String.Format(strpath, System.Web.HttpContext.Current.Server.MapPath("~"));
            string FILE_NAME = (strpath);
            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true);
            objWriter.WriteLine(System.Environment.NewLine + "************************************************************************" + System.Environment.NewLine + query);
            objWriter.Close();
        }
        catch (Exception)
        {
            // MsgBox(ex.Message)
        }
        return "";
    }
    public string getCheckConsolidationAddress(string strDelDocNumber, string strERPCon)
    {
        string strFlag = "False";
        string checkcompany = "";
        string checkAddress1 = "";
        string checkAddress2 = "";
        string checkCity = "";
        string checkState = "";
        string checkCountry = "";
        string checkzipcode = "";

        try
        {
            if (DelInfo.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= DelInfo.Tables[0].Rows.Count; j++)
                {
                    if (j == 0)
                    {
                        checkcompany = DelInfo.Tables[0].Rows[j]["CompanyName"].ToString();
                        checkAddress1 = DelInfo.Tables[0].Rows[j]["AddressLine1"].ToString();
                        checkAddress2 = DelInfo.Tables[0].Rows[j]["AddressLine2"].ToString();
                        checkCity = DelInfo.Tables[0].Rows[j]["City"].ToString();
                        checkState = DelInfo.Tables[0].Rows[j]["State"].ToString();
                        checkCountry = DelInfo.Tables[0].Rows[j]["Country"].ToString();
                        checkzipcode = DelInfo.Tables[0].Rows[j]["ZipCode"].ToString();
                    }
                    else
                    {
                        if ((checkcompany == DelInfo.Tables[0].Rows[j]["CompanyName"].ToString()) && (checkAddress1 == DelInfo.Tables[0].Rows[j]["AddressLine1"].ToString()) && (checkAddress2 == DelInfo.Tables[0].Rows[j]["AddressLine2"].ToString()) && (checkCity == DelInfo.Tables[0].Rows[j]["City"].ToString()) && (checkState == DelInfo.Tables[0].Rows[j]["State"].ToString()) && (checkCountry == DelInfo.Tables[0].Rows[j]["Country"].ToString()) && (checkzipcode == DelInfo.Tables[0].Rows[j]["ZipCode"].ToString()))
                        {
                        }
                        else
                        {
                            strFlag = "True";
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }
        return strFlag;
    }
    public string GetShipFromDetails()
    {
        string result = string.Empty;
        string StrShipfromQuery = string.Empty;
        bool sfmflag = false;       
        try
        {
            if (BillToCode == "BES100-DS" || BillToCode == "BES101-DS" && strAccountNumber != string.Empty)
            {
                sfmflag = true;
                StrShipfromQuery = "select * from ShipFromAccounts where PlantID = '" + PlantID + "' and AccountNumber = '" + strAccountNumber + "' and  customerCodes like '%" + BillToCode + "%'";
            }
            else if (BillToCode == "SOU600-DS" && strAccountNumber != string.Empty)
            {
                sfmflag = true;
                StrShipfromQuery = "select * from ShipFromAccounts where PlantID = '" + PlantID + "' and AccountNumber = '" + strAccountNumber + "' and  customerCodes like '%" + BillToCode + "%'";
            }
            else
            {
                StrShipfromQuery = "Select * from XCARRIER_LOCATION where PLANT_ID='" + PlantID + "' and ACTIVE_PLANT='True'";
            }
            addquery("ShipFrom" + StrShipfromQuery);
            SqlDataAdapter da = new SqlDataAdapter(StrShipfromQuery, SqlCon);
            DataSet ds = new DataSet();
            da.Fill(ds);

            result = "<ShipFromAddress>" + System.Environment.NewLine;
            if (ds.Tables[0].Rows.Count > 0)
            {
                try
                {
                    if (sfmflag == true)
                    {
                        result = result + "<sPlantCompany>" + ds.Tables[0].Rows[0]["Company"].ToString() + "</sPlantCompany>" + System.Environment.NewLine;
                        result = result + "<sPlantContact>" + ds.Tables[0].Rows[0]["Contact"].ToString() + "</sPlantContact>" + System.Environment.NewLine;
                        result = result + "<sPlantAddressLine1>" + ds.Tables[0].Rows[0]["AddressLine1"].ToString() + "</sPlantAddressLine1>" + System.Environment.NewLine;
                        result = result + "<sPlantAddressLine2>" + ds.Tables[0].Rows[0]["AddressLine2"].ToString() + "</sPlantAddressLine2>" + System.Environment.NewLine;
                        result = result + "<sPlantCity>" + ds.Tables[0].Rows[0]["City"].ToString() + "</sPlantCity>" + System.Environment.NewLine;
                        result = result + "<sPlantState>" + ds.Tables[0].Rows[0]["State"].ToString() + "</sPlantState>" + System.Environment.NewLine;
                        result = result + "<sPlantCountry>" + ds.Tables[0].Rows[0]["Country"].ToString() + "</sPlantCountry>" + System.Environment.NewLine;
                        result = result + "<sPlantZipCode>" + ds.Tables[0].Rows[0]["ZipCode"].ToString() + "</sPlantZipCode>" + System.Environment.NewLine;
                        result = result + "<sPlantPhone>" + ds.Tables[0].Rows[0]["Phone"].ToString() + "</sPlantPhone>" + System.Environment.NewLine;
                        result = result + "<sPlantEmail>" + ds.Tables[0].Rows[0]["Email"].ToString() + "</sPlantEmail>" + System.Environment.NewLine;

                        result = result + "<sPlantWeightUnits>LBS</sPlantWeightUnits>" + System.Environment.NewLine;
                        result = result + "<sPlantCurrency>CAD</sPlantCurrency>" + System.Environment.NewLine;
                        result = result + "<sPlantDimUnits>IN</sPlantDimUnits>" + System.Environment.NewLine;
                    }
                    else
                    {
                        result = result + "<sPlantCompany>" + ds.Tables[0].Rows[0]["COMPANY"].ToString() + "</sPlantCompany>" + System.Environment.NewLine;
                        result = result + "<sPlantContact>" + ds.Tables[0].Rows[0]["CONTACT"].ToString() + "</sPlantContact>" + System.Environment.NewLine;
                        result = result + "<sPlantAddressLine1>" + ds.Tables[0].Rows[0]["ADDRESS_LINE1"].ToString() + "</sPlantAddressLine1>" + System.Environment.NewLine;
                        result = result + "<sPlantAddressLine2>" + ds.Tables[0].Rows[0]["ADDRESS_LINE2"].ToString() + "</sPlantAddressLine2>" + System.Environment.NewLine;
                        result = result + "<sPlantCity>" + ds.Tables[0].Rows[0]["CITY"].ToString() + "</sPlantCity>" + System.Environment.NewLine;
                        result = result + "<sPlantState>" + ds.Tables[0].Rows[0]["STATE"].ToString() + "</sPlantState>" + System.Environment.NewLine;
                        result = result + "<sPlantCountry>" + ds.Tables[0].Rows[0]["COUNTRY"].ToString() + "</sPlantCountry>" + System.Environment.NewLine;
                        result = result + "<sPlantZipCode>" + ds.Tables[0].Rows[0]["ZIPCODE"].ToString() + "</sPlantZipCode>" + System.Environment.NewLine;
                        result = result + "<sPlantPhone>" + ds.Tables[0].Rows[0]["PHONE"].ToString() + "</sPlantPhone>" + System.Environment.NewLine;
                        result = result + "<sPlantEmail>" + ds.Tables[0].Rows[0]["EMAIL"].ToString() + "</sPlantEmail>" + System.Environment.NewLine;

                        result = result + "<sPlantWeightUnits>" + ds.Tables[0].Rows[0]["WEIGHT"].ToString() + "</sPlantWeightUnits>" + System.Environment.NewLine;
                        result = result + "<sPlantCurrency>" + ds.Tables[0].Rows[0]["CURRENCY_CODE"].ToString() + "</sPlantCurrency>" + System.Environment.NewLine;
                        result = result + "<sPlantDimUnits>" + ds.Tables[0].Rows[0]["DIMENSION"].ToString() + "</sPlantDimUnits>" + System.Environment.NewLine;
                    }
                    
                }
                catch (Exception)
                {                   
                    result = result + "<sPlantCompany></sPlantCompany>" + System.Environment.NewLine;
                    result = result + "<sPlantContact></sPlantContact>" + System.Environment.NewLine;
                    result = result + "<sPlantAddressLine1></sPlantAddressLine1>" + System.Environment.NewLine;
                    result = result + "<sPlantAddressLine2></sPlantAddressLine2>" + System.Environment.NewLine;
                    result = result + "<sPlantCity></sPlantCity>" + System.Environment.NewLine;
                    result = result + "<sPlantState></sPlantState>" + System.Environment.NewLine;
                    result = result + "<sPlantCountry></sPlantCountry>" + System.Environment.NewLine;
                    result = result + "<sPlantZipCode></sPlantZipCode>" + System.Environment.NewLine;
                    result = result + "<sPlantPhone></sPlantPhone>" + System.Environment.NewLine;
                    result = result + "<sPlantEmail></sPlantEmail>" + System.Environment.NewLine;
                    result = result + "<sPlantWeightUnits>LBS</sPlantWeightUnits>" + System.Environment.NewLine;
                    result = result + "<sPlantCurrency>CAD</sPlantCurrency>" + System.Environment.NewLine;
                    result = result + "<sPlantDimUnits>IN</sPlantDimUnits>" + System.Environment.NewLine;
                }
            }
            result = result + "</ShipFromAddress>" + System.Environment.NewLine;
            return result;
        }
        catch (Exception)
        {
            result = "<ShipFromAddress></ShipFromAddress>" + System.Environment.NewLine;
            return result;
        }       
    }

    public string GetAdditionalInfo()
    {
        string result = string.Empty;
        try
        {
            result = result + "<AdditionalInfo>" + System.Environment.NewLine;
            if (BillToCode == "BES100-DS" || BillToCode == "BES101-DS" && sCarrierMethod.ToUpper() == "CANADAPOST")
            {                
                result = result + "<BBY>TRUE</BBY>" + System.Environment.NewLine;   
            }
            else
            {
                result = result + "<BBY>FALSE</BBY>" + System.Environment.NewLine;
            }
            result = result + "</AdditionalInfo>" + System.Environment.NewLine;
        }
        catch (Exception)
        {
            result = result + "<AdditionalInfo>" + System.Environment.NewLine;            
            result = result + "<BBY>FALSE</BBY>" + System.Environment.NewLine;
            result = result + "</AdditionalInfo>" + System.Environment.NewLine;
        }
        return result;
    }
    public string GetShipToDetails()
    {
        string result = "";
        string StrShipToQuery = string.Empty;        
        try
        {
            result = "<ShipToAddress>";
            if (DelInfo.Tables[0].Rows.Count > 0)
            {
                sShipToCompany = DelInfo.Tables[0].Rows[0]["CompanyName"].ToString();
                sShipToContact = DelInfo.Tables[0].Rows[0]["ContactPersonName"].ToString();
                sShipToAddressLine1 = DelInfo.Tables[0].Rows[0]["AddressLine1"].ToString();
                sShipToAddressLine2 = DelInfo.Tables[0].Rows[0]["AddressLine2"].ToString();
                sShipToAddressLine3 = DelInfo.Tables[0].Rows[0]["AddressLine3"].ToString();
                sShipToCity = DelInfo.Tables[0].Rows[0]["City"].ToString();
                sShipToState = DelInfo.Tables[0].Rows[0]["State"].ToString();
                sShipToCountry = DelInfo.Tables[0].Rows[0]["Country"].ToString();
                sShipToZipCode = DelInfo.Tables[0].Rows[0]["ZipCode"].ToString();
                sShipToPhone = DelInfo.Tables[0].Rows[0]["PhoneNumber"].ToString();
                sShipToEmail = DelInfo.Tables[0].Rows[0]["Email"].ToString();
                BillToCode = DelInfo.Tables[0].Rows[0]["BILLTO"].ToString();
                ShipViaName = DelInfo.Tables[0].Rows[0]["ShipviaCode"].ToString();
                sInvoiceNo = DelInfo.Tables[0].Rows[0]["InvoiceNum"].ToString();
                sPONo = DelInfo.Tables[0].Rows[0]["PoNum"].ToString();

                if (sShipToContact == "")
                {
                    sShipToContact = "Receiver";
                }

                result = result + "<Company>" + removeSplcharactersXML(sShipToCompany) + "</Company>" + System.Environment.NewLine;
                result = result + "<Contact>" + removeSplcharactersXML(sShipToContact) + "</Contact>" + System.Environment.NewLine;
                result = result + "<AddressLine1>" + removeSplcharactersXML(sShipToAddressLine1) + "</AddressLine1>" + System.Environment.NewLine;
                result = result + "<AddressLine2>" + removeSplcharactersXML(sShipToAddressLine2) + "</AddressLine2>" + System.Environment.NewLine;
                result = result + "<AddressLine3>" + removeSplcharactersXML(sShipToAddressLine3) + "</AddressLine3>" + System.Environment.NewLine;
                result = result + "<City>" + removeSplcharactersXML(sShipToCity) + "</City>" + System.Environment.NewLine;
                result = result + "<State>" + removeSplcharactersXML(sShipToState) + "</State>" + System.Environment.NewLine;
                result = result + "<Country>" + removeSplcharactersXML(sShipToCountry) + "</Country>" + System.Environment.NewLine;
                result = result + "<ZipCode>" + removeSplcharactersXML(sShipToZipCode) + "</ZipCode>" + System.Environment.NewLine;
                result = result + "<Phone>" + removeSplcharactersXML(sShipToPhone) + "</Phone>" + System.Environment.NewLine;
                result = result + "<Email>" + removeSplcharactersXML(sShipToEmail) + "</Email>" + System.Environment.NewLine;
                result = result + "<strCarrierCode>" + ShipViaName + "</strCarrierCode>" + System.Environment.NewLine;
                result = result + "<PONo>" + removeSplcharactersXML(sPONo) + "</PONo>" + System.Environment.NewLine;
                result = result + "<InvoiceNo>" + removeSplcharactersXML(sInvoiceNo) + "</InvoiceNo>" + System.Environment.NewLine;
            }
            result = result + "</ShipToAddress>" + System.Environment.NewLine;
            return result;
        }
        catch (Exception)
        {
            result = "<ShipToAddress></ShipToAddress>" + System.Environment.NewLine;
            return result;
        }
    }
    public string GetPackageInfo()
    {
        string result = string.Empty;
        
        double TotalPackWeight = 0.00;
        try
        {            
            result = "<PACKAGEINFO>" + System.Environment.NewLine;
            for (int i = 0; i <= strDelNo.Length - 1; i++)
            {
                string strqueryPack = "select * from [PWXCR_DEL_INFO] where SSCCnum in(" + strDelNo[i] + ") order by SSCCnum";
                addquery(strqueryPack);
                SqlDataAdapter SqldaPack = new SqlDataAdapter(strqueryPack, ERPCon);
                DataSet DsPack = new DataSet();
                SqldaPack.Fill(DsPack);
                result = result + "<LPN>" + System.Environment.NewLine;
                result = result + "<LPN_NUMBER>" + strDelNo[i] + "</LPN_NUMBER>" + System.Environment.NewLine;
                if (DsPack.Tables[0].Rows.Count > 0)
                {
                    double Packweight = 0.00;
                    for (int j = 0; j <= DsPack.Tables[0].Rows.Count - 1; j++)
                    {
                        //------------------------------ Items Info-------------------------------
                        result = result + "<Item>" + System.Environment.NewLine;
                        result = result + "<ItemNo>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["ItemNumber"].ToString()) + "</ItemNo>" + System.Environment.NewLine;
                        result = result + "<SerialNo>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["SSCCNum"].ToString()) + "</SerialNo>" + System.Environment.NewLine;
                        result = result + "<Description>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["ItemDescription"].ToString()) + "</Description>" + System.Environment.NewLine;
                        result = result + "<Quantity>" + RemoveSplcharXML(removeSplcharactersXML(DsPack.Tables[0].Rows[j]["ItemQuantity"].ToString())) + "</Quantity>" + System.Environment.NewLine;
                        result = result + "<ItemWeight>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["ItemWeight"].ToString()) + "</ItemWeight>" + System.Environment.NewLine;
                        result = result + "</Item>" + System.Environment.NewLine;

                        result = result + "<Dimensions>" + System.Environment.NewLine;
                        result = result + "<Length>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["Length"].ToString().Trim()) + "</Length>" + System.Environment.NewLine;
                        result = result + "<Width>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["Width"].ToString().Trim()) + "</Width>" + System.Environment.NewLine;
                        result = result + "<Height>" + removeSplcharactersXML(DsPack.Tables[0].Rows[j]["Height"].ToString().Trim()) + "</Height>" + System.Environment.NewLine;
                        result = result + "</Dimensions>" + System.Environment.NewLine;
                        try
                        {
                            Packweight = Convert.ToDouble(DsPack.Tables[0].Rows[j]["ItemWeight"].ToString());
                            TotalPackWeight = Convert.ToDouble(DsPack.Tables[0].Rows[j]["ItemWeight"].ToString());
                        }
                        catch (Exception Ex1)
                        {
                            addquery(Ex1.Message.ToString());
                        }
                    }
                    result = result + "<WEIGHT>" + Packweight.ToString("#0.00") + "</WEIGHT>" + System.Environment.NewLine;
                }
                result = result + "</LPN>" + System.Environment.NewLine;                
            }
            result = result + "<TOTALPACKWEIGHT>" + TotalPackWeight.ToString("#0.00") + "</TOTALPACKWEIGHT>" + System.Environment.NewLine;
            result = result + "</PACKAGEINFO>" + System.Environment.NewLine;
            return result;
        }
        catch (Exception Ex2)
        {
            addquery(Ex2.Message.ToString());
            result = "<PACKAGEINFO></PACKAGEINFO>" + System.Environment.NewLine;
            return result;
        }
    }
    public string GetItemsInfo()
    {
        string result = string.Empty;
        try
        {
            //string strItemsQuery = "select * from [PWXCR_DEL_INFO] where SSCCnum in(" + strDelivery + ") order by SSCCnum";
            //addquery("Items " + strItemsQuery);

            //SqlDataAdapter SqldaItems = new SqlDataAdapter(strItemsQuery, ERPCon);
            //DataSet dsItems = new DataSet();
            //SqldaItems.Fill(dsItems);
            if (DelInfo.Tables[0].Rows.Count > 0)
            {
                result = "<Items>" + System.Environment.NewLine;
                for (int i = 0; i <= DelInfo.Tables[0].Rows.Count - 1; i++)
                {
                    result = result + "<Item>" + System.Environment.NewLine;
                    result = result + "<ItemNo>" + removeSplcharactersXML(DelInfo.Tables[0].Rows[i]["ItemNumber"].ToString()) + "</ItemNo>" + System.Environment.NewLine;
                    result = result + "<SerialNo>" + removeSplcharactersXML(DelInfo.Tables[0].Rows[i]["SSCCNum"].ToString()) + "</SerialNo>" + System.Environment.NewLine;
                    result = result + "<ItemDescription>" + removeSplcharactersXML(DelInfo.Tables[0].Rows[i]["ItemDescription"].ToString()) + "</ItemDescription>" + System.Environment.NewLine;
                    result = result + "<ItemQuantity>" + removeSplcharactersXML(DelInfo.Tables[0].Rows[i]["ItemQuantity"].ToString()) + "</ItemQuantity>" + System.Environment.NewLine;
                    result = result + "</Item>" + System.Environment.NewLine;
                }
                result = result + "</Items>" + System.Environment.NewLine;
            }
            return result;
        }
        catch (Exception)
        {
           return result= "<Items></Items>" + System.Environment.NewLine;
        }        
    }
    public string GetServiceCodeByServiceName(string Srv,string crr)
    {
        string strQueryservcode = string.Empty;
        string serviceCode = string.Empty;
        try
        {
            strQueryservcode = "select SERVICE_LEVEL_CODE from XCARRIER_SERVICE_MASTER where SERVICE_LEVEL_NAME='" + Srv + "' and CARRIER='" + crr + "'";
            addquery("Carriers " + strQueryservcode);
            SqlDataAdapter sqldasrvcode = new SqlDataAdapter(strQueryservcode, SqlCon);
            DataSet dssrvcode = new DataSet();
            sqldasrvcode.Fill(dssrvcode);
            if (dssrvcode.Tables[0].Rows.Count > 0)
            {
                serviceCode = dssrvcode.Tables[0].Rows[0]["SERVICE_LEVEL_CODE"].ToString();
            }
            return serviceCode;
        }
        catch (Exception)
        {
            return serviceCode;
        }       
    }
    public string GetCarrierInfo()
    {
        string result = string.Empty;
        string strqueryCrrAccounts = string.Empty;
        try
        {
            string strvmcarrier1 = "select Carrier,Service,PaymentType from ERP_MarkUpValues where shipviacode='" + ShipViaName + "'";
            //string strvmcarrier1 = "select shipviacode,Carrier,Service,Payment,CODFlag,SatFlag from ShipViaCodes where shipviacode='" + ShipViaName + "'";
            addquery("Carriers " + strvmcarrier1);
            SqlDataAdapter sqldavmcarrier1 = new SqlDataAdapter(strvmcarrier1, SqlCon);
            DataSet dsvmcarrier1 = new DataSet();
            sqldavmcarrier1.Fill(dsvmcarrier1);

            result = "<CarrierInfo>";
            if (dsvmcarrier1.Tables[0].Rows.Count > 0)
            {
                sCarrierMethod = dsvmcarrier1.Tables[0].Rows[0]["Carrier"].ToString();
                sCarrierService = dsvmcarrier1.Tables[0].Rows[0]["Service"].ToString();
                sCarrierPaymentMethod = dsvmcarrier1.Tables[0].Rows[0]["PaymentType"].ToString();
                CanadaPostPayorAccountNumber = strAccountNumber;
                try
                {
                    if (dsvmcarrier1.Tables[0].Rows[0]["CODFlag"].ToString() == "True")//signature required option for canada post 
                    {
                        strCODFlag = "1";
                    }
                    if (dsvmcarrier1.Tables[0].Rows[0]["SatFlag"].ToString() == "True")//email notification for all carriers
                    {
                        strSatFlag = "1";
                    }
                }
                catch (Exception)
                {

                }               
                result = result + "<Carrier>" + removeSplcharactersXML(sCarrierMethod.ToString().ToUpper()) + "</Carrier>" + System.Environment.NewLine;
                result = result + "<Service>" + removeSplcharactersXML(GetServiceCodeByServiceName(sCarrierService,sCarrierMethod)) + "</Service>" + System.Environment.NewLine;
                result = result + "<CarrierName>" + removeSplcharactersXML(sCarrierMethod.ToString()) + "</CarrierName>" + System.Environment.NewLine;
                result = result + "<ServiceName>" + removeSplcharactersXML(sCarrierService.ToString()) + "</ServiceName>" + System.Environment.NewLine;                
                result = result + "<Payment>" + removeSplcharactersXML(sCarrierPaymentMethod) + "</Payment>" + System.Environment.NewLine;
                result = result + "<CODFlag>" + strCODFlag + "</CODFlag>" + System.Environment.NewLine;
                result = result + "<SatFlag>" + strSatFlag + "</SatFlag>" + System.Environment.NewLine;
                result = result + "<PONo>" + removeSplcharactersXML(sPONo) + "</PONo>" + System.Environment.NewLine;
                result = result + "<InvoiceNo>" + removeSplcharactersXML(sInvoiceNo) + "</InvoiceNo>" + System.Environment.NewLine;

                try
                {
                    if (sCarrierMethod.ToUpper() == "CANADAPOST")
                    {
                        if (BillToCode == "BES100-DS")
                        {
                            strAccountNumber = "2000301";
                            CanadaPostPayorAccountNumber = "0007309303";
                        }
                        if (BillToCode == "BES101-DS")
                        {
                            strAccountNumber = "2000301";
                            CanadaPostPayorAccountNumber = "0007309303";
                        }
                        if (BillToCode == "SOU600-DS")
                        {
                            strAccountNumber = "1000056";
                            CanadaPostPayorAccountNumber = "0007317295";
                        }
                    }
                    else if (sCarrierMethod.ToUpper() == "PUROLATORESHIP" || sCarrierMethod.ToUpper() == "PUROLATOR")
                    {
                        if (BillToCode == "BES100-DS")
                        {
                            strAccountNumber = "6271561";
                            sThirdpartyaccountno = "8063545";
                        }
                        if (BillToCode == "BES101-DS")
                        {
                            strAccountNumber = "6271561";
                            sThirdpartyaccountno = "8063545";
                        }
                    }
                    else if (sCarrierMethod.ToUpper() == "UPS")
                    {
                        if (BillToCode == "SOU600-DS")
                        {
                            strAccountNumber = "WE9441";
                        }
                    }
                    else
                    {
                        result = result + "<PaymentAccount>" + removeSplcharactersXML(sThirdpartyaccountno) + "</PaymentAccount>" + System.Environment.NewLine;
                        result = result + "<CanadaPostPayorAccountNumber>" + CanadaPostPayorAccountNumber + "</CanadaPostPayorAccountNumber>" + System.Environment.NewLine;
                        result = result + "<PaymentZipCode>" + removeSplcharactersXML(sShipToThirdPartyZipCode) + "</PaymentZipCode>" + System.Environment.NewLine;
                        result = result + "<PaymentCountry>" + removeSplcharactersXML(sShipToThirdPartyCountry) + "</PaymentCountry>" + System.Environment.NewLine;
                    }

                    if (sCarrierPaymentMethod.ToUpper() != "SENDER")
                    {
                        result = result + "<PaymentAccount>" + sThirdpartyaccountno + "</PaymentAccount>" + System.Environment.NewLine;
                        result = result + "<CanadaPostPayorAccountNumber>" + CanadaPostPayorAccountNumber + "</CanadaPostPayorAccountNumber>" + System.Environment.NewLine;
                        result = result + "<PaymentZipCode>" + removeSplcharactersXML(sShipToZipCode) + "</PaymentZipCode>" + System.Environment.NewLine;
                        result = result + "<PaymentCountry>" + removeSplcharactersXML(sShipToCountry) + "</PaymentCountry>" + System.Environment.NewLine;
                    }
                    else
                    {
                        result = result + "<PaymentAccount>" + strAccountNumber + "</PaymentAccount>" + System.Environment.NewLine;
                        result = result + "<CanadaPostPayorAccountNumber>" + CanadaPostPayorAccountNumber + "</CanadaPostPayorAccountNumber>" + System.Environment.NewLine;
                        result = result + "<PaymentZipCode>" + removeSplcharactersXML(sShipToThirdPartyZipCode) + "</PaymentZipCode>" + System.Environment.NewLine;
                        result = result + "<PaymentCountry>" + removeSplcharactersXML(sShipToThirdPartyCountry) + "</PaymentCountry>" + System.Environment.NewLine;
                    }

                }
                catch (Exception)
                {
                    result = result + "<PaymentAccount></PaymentAccount>" + System.Environment.NewLine;
                    result = result + "<PaymentZipCode></PaymentZipCode>" + System.Environment.NewLine;
                    result = result + "<PaymentCountry></PaymentCountry>" + System.Environment.NewLine;
                }
                result = result + "<strUPSResFlag></strUPSResFlag>" + System.Environment.NewLine;

                //------------------------------------ Carrier Default Accounts ---------------------------------------------//              

                try
                {
                    if (strAccountNumber != "")
                    {
                        strqueryCrrAccounts = "select * from XCARRIER_CARRIER where CARRIER='" + sCarrierMethod + "' and ACCOUNT_NUMBER='" + strAccountNumber + "' and PLANT_ID='" + PlantID + "'";
                    }
                    else
                    {
                        strqueryCrrAccounts = "select * from XCARRIER_CARRIER where CARRIER='" + sCarrierMethod + "' and DEFAULT_ACCOUNTNUMBER='TRUE' and PLANT_ID='" + PlantID + "'";
                    }
                    addquery("CarrierAccounst " + strqueryCrrAccounts);

                    SqlDataAdapter sqldaCrrAcc = new SqlDataAdapter(strqueryCrrAccounts, SqlCon);
                    DataSet dsCrrAcc = new DataSet();
                    sqldaCrrAcc.Fill(dsCrrAcc);

                    result = result + "<CarrierAccounts>" + System.Environment.NewLine;
                    if (dsCrrAcc.Tables[0].Rows.Count > 0)
                    {
                        result = result + "<AccountNumber>" + dsCrrAcc.Tables[0].Rows[0]["ACCOUNT_NUMBER"].ToString() + "</AccountNumber>" + System.Environment.NewLine;
                        result = result + "<UserID>" + dsCrrAcc.Tables[0].Rows[0]["USER_ID"].ToString() + "</UserID>" + System.Environment.NewLine;
                        result = result + "<Password>" + dsCrrAcc.Tables[0].Rows[0]["PASSWORD"].ToString() + "</Password>" + System.Environment.NewLine;
                        result = result + "<LicenseNumber>" + dsCrrAcc.Tables[0].Rows[0]["LICENSE_NUMBER"].ToString() + "</LicenseNumber>" + System.Environment.NewLine;
                        result = result + "<MeterNumber>" + dsCrrAcc.Tables[0].Rows[0]["METER_NUMBER"].ToString() + "</MeterNumber>" + System.Environment.NewLine;
                        result = result + "<ShippingKey>" + dsCrrAcc.Tables[0].Rows[0]["SHIPPINGKEY"].ToString() + "</ShippingKey>" + System.Environment.NewLine;
                        //result = result + "<TariffName>" + dsCrrAcc.Tables[0].Rows[0]["TariffName"].ToString() + "</TariffName>" + System.Environment.NewLine;
                    }
                    result = result + "</CarrierAccounts>" + System.Environment.NewLine;
                }
                catch (Exception)
                {
                    result = result + "<CarrierAccounts></CarrierAccounts>" + System.Environment.NewLine;
                }                
            }
            result = result + "</CarrierInfo>" + System.Environment.NewLine;

            return result;
        }
        catch (Exception)
        {
            return "<CarrierInfo></CarrierInfo>";
        }
    }
    public string importgetdelivery(string strReqXML)
    {
        string result = "";
        try
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(strReqXML);
                       
            string ERPName = "";  
            string DeliveryNo = "";

            PlantID = xmldoc.GetElementsByTagName("PlantID")[0].InnerText;
            DeliveryNo = xmldoc.GetElementsByTagName("DeliveryNo")[0].InnerText;
            ERPName = xmldoc.GetElementsByTagName("ERPName")[0].InnerText;
            CustomerID = xmldoc.GetElementsByTagName("CUSTOMERID")[0].InnerText;
            ERPCon = GetConnection(PlantID, ERPName, CustomerID);
            SqlConnection sqlErpCon = new SqlConnection(ERPCon);

            strDelNo = DeliveryNo.Split(',');
            strDelivery = "";
            for (int i = 0; i <= strDelNo.Length - 1; i++)
            {
                if (i == strDelNo.Length - 1)
                {
                    strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "'";
                }
                else if (i == 0)
                {
                    strDelivery = "'" + strDelNo[i].ToString() + "',";
                }
                else
                {
                    strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "',";
                }
            }

            if (xmldoc.GetElementsByTagName("Operation")[0].InnerText == "Import")
            {
                //Delivery # checking for shipped status
                try
                {
                    string shipcheck = string.Empty;
                    shipcheck = "select * from xcarriershipping where prono in (" + strDelivery + ") and status='Shipped'";
                    SqlDataAdapter dashipcheck = new SqlDataAdapter(shipcheck, ERPCon);
                    DataSet dsshipcheck = new DataSet();
                    dashipcheck.Fill(dsshipcheck);

                    if (dsshipcheck.Tables[0].Rows.Count > 0)
                    {
                        result = "<Responce>" + System.Environment.NewLine;
                        result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                        result = result + "<ERROR>" + System.Environment.NewLine;
                        result = result + "<ERRORMESSAGE>SSCC # Already Shipped..!</ERRORMESSAGE>" + System.Environment.NewLine;
                        result = result + "</ERROR>" + System.Environment.NewLine;
                        result = result + "</Responce>" + System.Environment.NewLine;
                        return result;
                    }
                    else
                    {
                        string strquery1 = "select * from [PWXCR_DEL_INFO] where SSCCnum in (" + strDelivery + ") order by SSCCnum";
                        addquery(strquery1);
                        SqlDataAdapter Sqlda1 = new SqlDataAdapter(strquery1, ERPCon);
                        Sqlda1.Fill(DelInfo);

                        if (DelInfo.Tables[0].Rows.Count <= 0)
                        {
                            result = "<Responce>" + System.Environment.NewLine;
                            result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                            result = result + "<ERROR>" + System.Environment.NewLine;
                            result = result + "<ERRORMESSAGE>SSCC # not found</ERRORMESSAGE>" + System.Environment.NewLine;
                            result = result + "</ERROR>" + System.Environment.NewLine;
                            result = result + "</Responce>" + System.Environment.NewLine;
                            return result;                            
                        }
                        if (strDelNo.Length > 0)
                        {
                            string strCheckAdd = getCheckConsolidationAddress(DeliveryNo, ERPCon);
                            if (strCheckAdd == "True")
                            {
                                result = "<Responce>" + System.Environment.NewLine;
                                result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                                result = result + "<ERROR>" + System.Environment.NewLine;
                                result = result + "<ERRORMESSAGE>Ship to Address does not match</ERRORMESSAGE>" + System.Environment.NewLine;
                                result = result + "</ERROR>" + System.Environment.NewLine;
                                result = result + "</Responce>" + System.Environment.NewLine;
                                return result;
                            }
                            else
                            {
                                result = "<DeliveryInfo>" + System.Environment.NewLine;

                                result = result + "<Status>SUCCESS</Status>" + System.Environment.NewLine;
                                result = result + "<DELIVERY_NUM>"+ DeliveryNo + "</DELIVERY_NUM>" + System.Environment.NewLine;
                                result = result + "<PLANTID>" + PlantID + "</PLANTID>" + System.Environment.NewLine;
                                result = result + "<CUSTOMERID>" + CustomerID + "</CUSTOMERID>" + System.Environment.NewLine;

                                //-------------------------------------- ShipTo Info -----------------------------------------------//
                                result = result + GetShipToDetails();

                                //-------------------------------------- Carrier Info --------------------------------------------//
                                result = result + GetCarrierInfo();

                                //-------------------------------------- Shipfrom Info ----------------------//
                                result = result + GetShipFromDetails();

                                //-------------------------------------- Get Additional Info ------------------//

                                result = result + GetAdditionalInfo();

                                //-------------------------------------- Item for Pack -----------------------------//
                                result = result + GetItemsInfo();

                                //-------------------------------------- Packages with Item -----------------------------------//
                                result = result + GetPackageInfo();

                                result = result + "</DeliveryInfo>";

                                try
                                {
                                    string mydocpath1 = "C:\\Processweaver\\BackUp";
                                    using (System.IO.StreamWriter outfile =
                                      new System.IO.StreamWriter(mydocpath1 + @"\ECSAPI_ImportDeliveryRes_"+ DeliveryNo + ".xml"))
                                    {
                                        outfile.Write(result);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = "<Responce>" + System.Environment.NewLine;
                    result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                    result = result + "<ERROR>" + System.Environment.NewLine;
                    result = result + "<ERRORMESSAGE>"+ ex.Message + "</ERRORMESSAGE>" + System.Environment.NewLine;
                    result = result + "</ERROR>" + System.Environment.NewLine;
                    result = result + "</Responce>" + System.Environment.NewLine;
                    return result;
                }
            }
            else if(xmldoc.GetElementsByTagName("Operation")[0].InnerText.ToString().ToUpper() == "CANCEL")
            {
                string cancarriername = string.Empty;
                string canpaymenttype = string.Empty;
                string canAccount = string.Empty;
                try
                {
                    string strcanquery1 = "select * from xCarrierShipping where prono in (" + strDelivery + ") and status='Shipped' and AccountNo!=''";
                    addquery(strcanquery1);
                    addquery(ERPCon);
                    SqlDataAdapter Sqlcanda1 = new SqlDataAdapter(strcanquery1, ERPCon);
                    DataSet cands1 = new DataSet();
                    Sqlcanda1.Fill(cands1);
                    if (cands1.Tables[0].Rows.Count > 0)
                    {
                        cancarriername = cands1.Tables[0].Rows[0]["Carrier"].ToString();
                        canpaymenttype = cands1.Tables[0].Rows[0]["payment"].ToString();
                        canAccount = cands1.Tables[0].Rows[0]["AccountNo"].ToString();
                    }

                    string strcanquery2 = "select * from XCARRIER_CARRIER where CARRIER='" + cancarriername + "' and ACCOUNT_NUMBER='" + canAccount + "' and Plant_id='" + PlantID + "'";
                    addquery(strcanquery2);
                    addquery(SqlCon.ConnectionString.ToString());
                    SqlDataAdapter Sqlcanda2 = new SqlDataAdapter(strcanquery2, SqlCon);
                    DataSet cands2 = new DataSet();
                    Sqlcanda2.Fill(cands2);

                    result = "<Result>";
                    if (cands2.Tables[0].Rows.Count > 0)
                    {
                        result = result + "<Status>SUCCESS</Status>" + System.Environment.NewLine;
                        result = result + "<Carrier>" + removeSplcharactersXML(cancarriername) + "</Carrier>" + System.Environment.NewLine;
                        result = result + "<AccountNo>" + removeSplcharactersXML(canAccount) + "</AccountNo>" + System.Environment.NewLine;
                        result = result + "<payment>" + removeSplcharactersXML(canpaymenttype) + "</payment>" + System.Environment.NewLine;                       
                        result = result + "<AccountNumber>" + cands2.Tables[0].Rows[0]["ACCOUNT_NUMBER"].ToString() + "</AccountNumber>" + System.Environment.NewLine;
                        result = result + "<UserID>" + cands2.Tables[0].Rows[0]["USER_ID"].ToString() + "</UserID>" + System.Environment.NewLine;
                        result = result + "<Password>" + cands2.Tables[0].Rows[0]["PASSWORD"].ToString() + "</Password>" + System.Environment.NewLine;
                        result = result + "<LicenseNumber>" + cands2.Tables[0].Rows[0]["LICENSE_NUMBER"].ToString() + "</LicenseNumber>" + System.Environment.NewLine;
                        result = result + "<MeterNumber>" + cands2.Tables[0].Rows[0]["METER_NUMBER"].ToString() + "</MeterNumber>" + System.Environment.NewLine;
                        result = result + "<ShippingKey>" + cands2.Tables[0].Rows[0]["SHIPPINGKEY"].ToString() + "</ShippingKey>" + System.Environment.NewLine;
                        //result = result + "<TariffName>" + cands2.Tables[0].Rows[0]["TariffName"].ToString() + "</TariffName>" + System.Environment.NewLine;
                        for (int pack = 0; pack <= cands1.Tables[0].Rows.Count-1; pack++)
                        {
                            result = result + "<Pack>" + System.Environment.NewLine;
                            result = result + "<TrackingNo>" + removeSplcharactersXML(cands1.Tables[0].Rows[pack]["trackingno"].ToString()) + "</TrackingNo>" + System.Environment.NewLine;
                            result = result + "<SerialNo>" + removeSplcharactersXML(cands1.Tables[0].Rows[pack]["prono"].ToString()) + "</SerialNo>" + System.Environment.NewLine;
                            result = result + "</Pack>" + System.Environment.NewLine;
                        }
                    }
                    else
                    {
                        result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                        result = result + "<ERROR>" + System.Environment.NewLine;
                        result = result + "<ERRORMESSAGE>No Data Found</ERRORMESSAGE>" + System.Environment.NewLine;
                        result = result + "</ERROR>" + System.Environment.NewLine;
                    }
                    result = result + "</Result>";
                }
                catch (Exception Ex)
                {
                    result = "<Responce>" + System.Environment.NewLine;
                    result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
                    result = result + "<ERROR>" + System.Environment.NewLine;
                    result = result + "<ERRORMESSAGE>"+ Ex.Message.ToString() + "</ERRORMESSAGE>" + System.Environment.NewLine;
                    result = result + "</ERROR>" + System.Environment.NewLine;
                    result = result + "</Responce>" + System.Environment.NewLine;
                    return result;
                }
            }
            return result;
        }
        catch (Exception ex111)
        {
            result = "<Responce>" + System.Environment.NewLine;
            result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
            result = result + "<ERROR>" + System.Environment.NewLine;
            result = result + "<ERRORMESSAGE>" + ex111.Message.ToString() + "</ERRORMESSAGE>" + System.Environment.NewLine;
            result = result + "</ERROR>" + System.Environment.NewLine;
            result = result + "</Responce>" + System.Environment.NewLine;
            return result;
        }        
    }
    public static DataTable GetDistinctRecords(DataTable dt, string Columns)
    {
        DataTable dtUniqRecords = new DataTable();
        dtUniqRecords = dt.DefaultView.ToTable(true, Columns);
        return dtUniqRecords;
    }
    public string exportgetdelivery(string strReqXML)
    {
        string result = "";
        try
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(strReqXML);            
            addquery("strReqXML" + strReqXML);
            if (xmldoc.GetElementsByTagName("Operation")[0].InnerText == "Export")
            {
                string ERPCon = string.Empty;
                string ERPName = string.Empty;
                string PlantID = string.Empty;
                string DeliveryNo = string.Empty;
                string strDelNoNew = string.Empty;
                string strshipcode = string.Empty;
                string strCustomerId = string.Empty;

                PlantID = xmldoc.GetElementsByTagName("PlantID")[0].InnerText;
                DeliveryNo = xmldoc.GetElementsByTagName("MultipleDeliveryNo")[0].InnerText;
                ERPName = xmldoc.GetElementsByTagName("ERPName")[0].InnerText;
                strCustomerId = xmldoc.GetElementsByTagName("CUSTOMERID")[0].InnerText;
                ERPCon = GetConnection(PlantID, ERPName,strCustomerId);
                strshipcode = xmldoc.GetElementsByTagName("UShipCode")[0].InnerText;
                SqlConnection sqlErpCon = new SqlConnection(ERPCon);

                string strDelivery = "";
                try
                {
                    string strquery5 = "SELECT DISTINCT ODLN.DocNum AS DELIVERYNUM FROM PMX_LUID INNER JOIN PMX_INVD ON PMX_INVD.\"LOGUNITIDENTKEY\" = PMX_LUID.\"INTERNALKEY\" JOIN OITM ON OITM.ITEMCODE = PMX_INVD.ITEMCODE JOIN DLN1 ON DLN1.DOCENTRY = PMX_INVD.DOCENTRY AND DLN1.LINENUM = PMX_INVD.DOCLINENUM JOIN ODLN ON ODLN.DOCENTRY = DLN1.DOCENTRY JOIN OCRD ON OCRD.CARDCODE = ODLN.CARDCODE JOIN CRD1 ON ODLN.SHIPTOCODE = CRD1.ADDRESS AND CRD1.ADRESTYPE = 'S' LEFT OUTER JOIN OSHP ON ODLN.TRNSPCODE=OSHP.TRNSPCODE WHERE CAST(PMX_LUID.SSCC AS BIGINT) IN(" + DeliveryNo + ") AND PMX_INVD.TRANSTYPE = '15'";
                    addquery(strquery5);
                    SqlDataAdapter Sqlda5 = new SqlDataAdapter(strquery5, ERPCon);
                    DataSet ds5 = new DataSet();
                    Sqlda5.Fill(ds5);

                    if (ds5.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i <= ds5.Tables[0].Rows.Count - 1; i++)
                        {
                            strDelNoNew = strDelNoNew + ds5.Tables[0].Rows[i]["DeliveryNum"].ToString() + ",";
                        }
                    }
                }
                catch (Exception)
                { }

                char[] MyChar = { ',' };
                string NewString = strDelNoNew.TrimEnd(MyChar);
                string[] strDelNo = NewString.Split(',');
                addquery(NewString);


                for (int i = 0; i <= strDelNo.Length - 1; i++)
                {

                    if (i == strDelNo.Length - 1)
                    {
                        strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "'";
                    }
                    else if (i == 0)
                    {
                        strDelivery = "'" + strDelNo[i].ToString() + "',";
                    }
                    else
                    {
                        strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "',";
                    }
                }

                for (int k = 0; k <= strDelNo.Length - 1; k++)
                {
                    double dblTotalFreight = 0.0;
                    try
                    {

                        SqlDataAdapter sqlDa1 = new SqlDataAdapter("select U_ECS_DOC_Freight from ODLN where docnum='" + strDelNo[k].ToString() + "'", sqlErpCon);
                        DataSet ds1 = new DataSet();
                        sqlDa1.Fill(ds1);
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                dblTotalFreight = Convert.ToDouble(ds1.Tables[0].Rows[0]["U_ECS_DOC_Freight"].ToString());
                            }
                            catch (Exception)
                            {
                                dblTotalFreight = 0.0;
                            }

                            try
                            {
                                dblTotalFreight = dblTotalFreight + double.Parse(xmldoc.GetElementsByTagName("DiscountedFreight")[0].InnerText);
                            }
                            catch (Exception)
                            {

                            }

                        }
                        try
                        {
                            SqlCommand SQLcmdUpdateODLN = new SqlCommand("update ODLN set u_ecs_doc_freight='" + dblTotalFreight.ToString("#0.00") + "' where docnum='" + strDelNo[k].ToString() + "'", sqlErpCon);
                            if (sqlErpCon.State == ConnectionState.Closed) { sqlErpCon.Open(); }
                            SQLcmdUpdateODLN.ExecuteNonQuery();
                            if (sqlErpCon.State == ConnectionState.Open) { sqlErpCon.Close(); }
                        }
                        catch (Exception)
                        { }
                        try
                        {
                            SqlCommand SQLcmdUpdateODLN = new SqlCommand("update ODLN set TrnspCode=(select TrnspCode from OSHP where TrnspName='" + strshipcode + "') where docnum='" + strDelNo[k].ToString() + "'", sqlErpCon);
                            addquery("Shipvia Update query : " + SQLcmdUpdateODLN.CommandText.ToString());
                            if (sqlErpCon.State == ConnectionState.Closed) { sqlErpCon.Open(); }
                            SQLcmdUpdateODLN.ExecuteNonQuery();
                            if (sqlErpCon.State == ConnectionState.Open) { sqlErpCon.Close(); }
                        }
                        catch (Exception)
                        {

                        }
                        DateTime dt;
                        try
                        {
                            dt = Convert.ToDateTime(xmldoc.GetElementsByTagName("ShipDate")[0].InnerText + " " + "00:00:00", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                        }
                        catch (Exception)
                        {
                            dt = DateTime.Now;
                        }

                        string strInserQuery = "";
                        try
                        {
                            XmlNodeList nodeList = xmldoc.SelectNodes("request/Pack");
                            for (int i = 0; i <= nodeList.Count - 1; i++)
                            {
                                if (i == 0)
                                {
                                    strInserQuery = "insert into xCarrierShipping(date,DelNo,Carrier,ShipMethod,Weight,shipcost,trackingno,Status,HandlingUnitid,center,prono,payment,AccountNo) values('" + xmldoc.GetElementsByTagName("ShipDate")[0].InnerText.Replace("'", "''") + "','" + strDelNo[k].ToString().Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Carrier")[0].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Service")[0].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Weight")[i].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("DiscountedFreight")[i].InnerText.ToString() + "','" + xmldoc.GetElementsByTagName("TRACKINGNO")[i].InnerText.ToString() + "','Shipped','" + xmldoc.GetElementsByTagName("HandlingUnitId")[i].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("DiscountedFreight")[i].InnerText.ToString() + "','" + xmldoc.GetElementsByTagName("CartonID")[0].InnerText + "','"+ xmldoc.GetElementsByTagName("PaymentType")[0].InnerText + "','" + xmldoc.GetElementsByTagName("AccountNumber")[0].InnerText + "')";
                                    addquery("Insert Query i=0 " + strInserQuery);
                                }
                                else
                                {
                                    strInserQuery = "insert into xCarrierShipping(date,DelNo,Carrier,ShipMethod,Weight,shipcost,trackingno,Status,HandlingUnitid,center,prono,payment,AccountNo) values('" + xmldoc.GetElementsByTagName("ShipDate")[0].InnerText.Replace("'", "''") + "','" + strDelNo[k].ToString().Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Carrier")[0].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Service")[0].InnerText.Replace("'", "''") + "','" + xmldoc.GetElementsByTagName("Weight")[i].InnerText.Replace("'", "''") + "','0.00','" + xmldoc.GetElementsByTagName("TRACKINGNO")[i].InnerText.ToString() + "','Shipped','" + xmldoc.GetElementsByTagName("HandlingUnitId")[i].InnerText.Replace("'", "''") + "','0.00','" + xmldoc.GetElementsByTagName("CartonID")[i].InnerText + "','" + xmldoc.GetElementsByTagName("PaymentType")[0].InnerText + "','" + xmldoc.GetElementsByTagName("AccountNumber")[0].InnerText + "')";
                                    addquery("Insert Query Ith" + i + "- Kth" + k+ "-" + strInserQuery);
                                }

                                SqlCommand SQLcmdInsert = new SqlCommand(strInserQuery, sqlErpCon);
                                if (sqlErpCon.State == ConnectionState.Closed) { sqlErpCon.Open(); }
                                SQLcmdInsert.ExecuteNonQuery();
                                if (sqlErpCon.State == ConnectionState.Open) { sqlErpCon.Close(); }
                            }
                        }
                        catch (Exception ex)
                        {
                            addquery("Insert Query " + strInserQuery);
                            addquery("Exception at Inserting " + ex.Message);
                            sqlErpCon.Close();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                result = "<Responce>" + System.Environment.NewLine;
                result = result + "<Status>SUCCESS</Status>" + System.Environment.NewLine;
                result = result + "<DATA>Export processed successfully..!</DATA>" + System.Environment.NewLine;
                result = result + "</Responce>" + System.Environment.NewLine;

                try
                {
                    string mydocpath1 = "C:\\Processweaver\\BackUp";
                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\B1ExportdeliveryRes.xml"))
                    {
                        outfile.Write(result);
                    }
                }
                catch (Exception)
                {
                }
                return result;
            }
            else if (xmldoc.GetElementsByTagName("Operation")[0].InnerText == "Cancel")
            {
                string ERPCon = string.Empty;
                string ERPName = string.Empty;
                string PlantID = string.Empty;
                string DeliveryNo = string.Empty;
                string strDelNoNew = string.Empty;
                string strCustomerId = string.Empty;

                PlantID = xmldoc.GetElementsByTagName("PlantID")[0].InnerText;
                DeliveryNo = xmldoc.GetElementsByTagName("MultipleDeliveryNo")[0].InnerText;
                ERPName = xmldoc.GetElementsByTagName("ERPName")[0].InnerText;
                strCustomerId = xmldoc.GetElementsByTagName("CUSTOMERID")[0].InnerText;
                ERPCon = GetConnection(PlantID, ERPName,strCustomerId);
                SqlConnection sqlErpCon = new SqlConnection(ERPCon);

                string strDelivery = "";
                try
                {
                    string strquery5 = "SELECT DISTINCT ODLN.DocNum AS DELIVERYNUM FROM PMX_LUID INNER JOIN PMX_INVD ON PMX_INVD.\"LOGUNITIDENTKEY\" = PMX_LUID.\"INTERNALKEY\" JOIN OITM ON OITM.ITEMCODE = PMX_INVD.ITEMCODE JOIN DLN1 ON DLN1.DOCENTRY = PMX_INVD.DOCENTRY AND DLN1.LINENUM = PMX_INVD.DOCLINENUM JOIN ODLN ON ODLN.DOCENTRY = DLN1.DOCENTRY JOIN OCRD ON OCRD.CARDCODE = ODLN.CARDCODE JOIN CRD1 ON ODLN.SHIPTOCODE = CRD1.ADDRESS AND CRD1.ADRESTYPE = 'S' LEFT OUTER JOIN OSHP ON ODLN.TRNSPCODE=OSHP.TRNSPCODE WHERE CAST(PMX_LUID.SSCC AS BIGINT) IN(" + DeliveryNo + ") AND PMX_INVD.TRANSTYPE = '15'";

                    addquery("Query At CancelShip "+strquery5);
                    SqlDataAdapter Sqlda5 = new SqlDataAdapter(strquery5, ERPCon);
                    DataSet ds5 = new DataSet();
                    Sqlda5.Fill(ds5);
                    if (ds5.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i <= ds5.Tables[0].Rows.Count - 1; i++)
                        {
                            strDelNoNew = strDelNoNew + ds5.Tables[0].Rows[i]["DeliveryNum"].ToString() + ",";
                        }
                    }
                }
                catch (Exception)
                { }

                char[] MyChar = { ',' };
                string NewString = strDelNoNew.TrimEnd(MyChar);
                string[] strDelNo = NewString.Split(',');
                addquery(NewString);

                for (int i = 0; i <= strDelNo.Length - 1; i++)
                {

                    if (i == strDelNo.Length - 1)
                    {
                        strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "'";
                    }
                    else if (i == 0)
                    {
                        strDelivery = "'" + strDelNo[i].ToString() + "',";
                    }
                    else
                    {
                        strDelivery = strDelivery + "'" + strDelNo[i].ToString() + "',";
                    }
                }

                for (int k = 0; k <= strDelNo.Length - 1; k++)
                {
                    double dblTotalFreight = 0.0;
                    try
                    {
                        addquery("select U_ECS_DOC_Freight from ODLN where docnum='" + strDelNo[k].ToString() + "'");
                        SqlDataAdapter sqlDa1 = new SqlDataAdapter("select U_ECS_DOC_Freight from ODLN where docnum='" + strDelNo[k].ToString() + "'", sqlErpCon);
                        DataSet ds1 = new DataSet();
                        sqlDa1.Fill(ds1);

                        addquery("select shipcost from xCarrierShipping where delno = '" + strDelNo[k].ToString() + "'");
                        SqlDataAdapter sqlDa2 = new SqlDataAdapter("select shipcost from xCarrierShipping where delno='" + strDelNo[k].ToString() + "'", sqlErpCon);
                        DataSet ds2 = new DataSet();
                        sqlDa2.Fill(ds2);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            try
                            {
                                dblTotalFreight = Convert.ToDouble(ds1.Tables[0].Rows[0]["U_ECS_DOC_Freight"].ToString());
                            }
                            catch (Exception)
                            {
                                dblTotalFreight = 0.0;
                            }

                            try
                            {
                                dblTotalFreight = dblTotalFreight - double.Parse(ds2.Tables[0].Rows[0]["shipcost"].ToString());
                            }
                            catch (Exception)
                            { }
                        }

                        try
                        {
                            addquery("update ODLN set u_ecs_doc_freight='" + dblTotalFreight.ToString("#0.00") + "'  where docnum='" + strDelNo[k].ToString() + "'");
                            SqlCommand SQLcmdUpdateODLN = new SqlCommand("update ODLN set u_ecs_doc_freight='" + dblTotalFreight.ToString("#0.00") + "'  where docnum='" + strDelNo[k].ToString() + "'", sqlErpCon);
                            if (sqlErpCon.State == ConnectionState.Closed) { sqlErpCon.Open(); }
                            SQLcmdUpdateODLN.ExecuteNonQuery();
                            if (sqlErpCon.State == ConnectionState.Open) { sqlErpCon.Close(); }
                        }
                        catch (Exception)
                        { }

                        try
                        {
                            string strQuery = "";          
                           
                            strQuery = "update xCarrierShipping set status='Cancelled' , CancelDate='" + DateTime.Now + "'  where DelNo='" + strDelNo[k].ToString() + "'";

                            addquery("Query At CancelShip updation " + strQuery);

                            SqlCommand SQLcmdInsert = new SqlCommand(strQuery, sqlErpCon);
                            if (sqlErpCon.State == ConnectionState.Closed) { sqlErpCon.Open(); }
                            SQLcmdInsert.ExecuteNonQuery();
                            if (sqlErpCon.State == ConnectionState.Open) { sqlErpCon.Close(); }

                        }
                        catch (Exception)
                        { }
                    }
                    catch (Exception)
                    { }
                }

                result = "<Responce>" + System.Environment.NewLine;
                result = result + "<Status>SUCCESS</Status>" + System.Environment.NewLine;
                result = result + "<DATA>Cancel Export processed successfully..!</DATA>" + System.Environment.NewLine;
                result = result + "</Responce>" + System.Environment.NewLine;

                try
                {
                    string mydocpath1 = "C:\\Processweaver\\BackUp";
                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\B1CanceldeliveryRes.xml"))
                    {
                        outfile.Write(result);
                    }
                }
                catch (Exception)
                { }

                return result;
            }


        }
        catch (Exception ex111)
        {
            result = "<Responce>" + System.Environment.NewLine;
            result = result + "<Status>ERROR</Status>" + System.Environment.NewLine;
            result = result + "<ERROR>" + System.Environment.NewLine;
            result = result + "<ERRORMESSAGE>" + ex111.Message.ToString() + "</ERRORMESSAGE>" + System.Environment.NewLine;
            result = result + "</ERROR>" + System.Environment.NewLine;
            result = result + "</Responce>" + System.Environment.NewLine;
            return result;
        }
        return result;
    }
    public string beforesaveshipment(string strReqXML)
    {
        string result = "";
        return result;
    }
    public string aftersaveshipment(string strReqXML)
    {
        string result = "";
        return result;
    }
    public string beforegateway(string strReqXML)
    {
        string result = "";
        return result;
    }
    public string removeSplcharactersXML(string strValue)
    {
        // Added By Yughandar for remove the special characters in Shipto address fields

        strValue = strValue.Replace("&", "&amp;");
        strValue = strValue.Replace("<", "&lt;");
        strValue = strValue.Replace(">", "&gt;");
        strValue = strValue.Replace("\"", "&quot;");
        strValue = strValue.Replace("'", "&apos;");
        strValue = strValue.Replace("(", "");
        strValue = strValue.Replace(")", "");
        return strValue;
    }
    public string RemoveSplcharXML(string strValue)
    {
        strValue = strValue.Replace("-", "");
        return strValue;
    }
    public String GetConnection(string strPlantID, string strerpname,string strCustomerId)
    {
        String strResult = "", strServerName = "", strDatasourceName = "", strusername = "", strpassword = "", strconnectionstmt = "";
        try
        {
            try
            {
                SqlCommand SqlCmd = new SqlCommand("Select ServerName,DatasourceName,DatasourceUsername,DatasourcePwd from XCARRIER_ERPSETUP where PlantID='" + strPlantID + "' and Erpname='" + strerpname + "' and CUSTOMER_ID='" + strCustomerId + "'", SqlCon);
                if (SqlCon.State != ConnectionState.Open) { SqlCon.Open(); }
                SqlDataReader dr = SqlCmd.ExecuteReader();
                if (dr.Read())
                {
                    strServerName = dr[0].ToString();
                    strDatasourceName = dr[1].ToString();
                    strusername = dr[2].ToString();
                    strpassword = dr[3].ToString();
                }
                dr.Close();
                if (SqlCon.State == ConnectionState.Open) { SqlCon.Close(); }
            }
            catch (Exception)
            {
                if (SqlCon.State == ConnectionState.Open) { SqlCon.Close(); }
            }
            
            if (strusername == "" && strpassword == "")
            {
                strconnectionstmt = "MultipleActiveResultSets=True;Integrated Security=SSPI;Persist Security Info=False;Data Source=" + strServerName + ";" + " initial catalog=" + strDatasourceName + ";" + "User ID=" + strusername + ";" + "Password=" + strpassword;
            }
            else
            {
                strconnectionstmt = "MultipleActiveResultSets=True;Data Source=" + strServerName + ";" + " initial catalog=" + strDatasourceName + ";" + "User ID=" + strusername + ";" + "Password=" + strpassword;
            }
            if (strconnectionstmt != "")
            {
                strResult = strconnectionstmt;
            }
        }
        catch (Exception)
        {
        }
        return strResult;
    }
    public string aftergateway(string strReqXML)
    {
        string result = "";
        return result;
    }
}