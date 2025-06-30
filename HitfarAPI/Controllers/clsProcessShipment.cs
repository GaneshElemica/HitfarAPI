using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for clsProcessShipment
/// </summary>
public class clsProcessShipment
{
    #region VariablesDeclaration
    public String[] arrTrackingNumber;
    public String[] arrFreight;
    public String[] arrDiscountFreight;
    public Double dblTotalFreight = 0.00;
    public Double discountTotalFreight = 0.00;
    public string PlantID = "1";
    public string DefaltERP = "SAP BUSINESS ONE";
    public string CUSTOMERID = "9243700001";
    #endregion
    public clsProcessShipment()
    {
    }
    public string PrepareGetDelxml(string ERP, string Delivery)
    {
        string RequestXML = "";
        RequestXML = "<request>";
        RequestXML = RequestXML + "<PlantID>" + PlantID + "</PlantID>";
        RequestXML = RequestXML + "<CUSTOMERID>" + CUSTOMERID + "</CUSTOMERID>";
        RequestXML = RequestXML + "<ERPName>" + DefaltERP + "</ERPName>";
        RequestXML = RequestXML + "<DeliveryNo>" + Delivery + "</DeliveryNo>";
        RequestXML = RequestXML + "<Operation>Import</Operation>";
        RequestXML = RequestXML + "</request>";

        return RequestXML;
    }
    public string PrepareGetDelCancelxml(string ERP, string Delivery)
    {
        string RequestXML = "";
        RequestXML = "<request>";
        RequestXML = RequestXML + "<PlantID>" + PlantID + "</PlantID>";
        RequestXML = RequestXML + "<CUSTOMERID>" + CUSTOMERID + "</CUSTOMERID>";
        RequestXML = RequestXML + "<ERPName>" + DefaltERP + "</ERPName>";
        RequestXML = RequestXML + "<DeliveryNo>" + Delivery + "</DeliveryNo>";
        RequestXML = RequestXML + "<Operation>CANCEL</Operation>";
        RequestXML = RequestXML + "</request>";

        return RequestXML;
    }
    public string PrepareGetExportxml(XmlDocument ShipmentRes, string DeliveryNo, XmlDocument MyXml)
    {
        string RequestXML = "";

        RequestXML = "<request>";
        RequestXML = RequestXML + "<Operation>Export</Operation>";
        RequestXML = RequestXML + "<PlantID>" + PlantID + "</PlantID>";
        RequestXML = RequestXML + "<ERPName>" + DefaltERP + "</ERPName>";
        RequestXML = RequestXML + "<CUSTOMERID>" + CUSTOMERID + "</CUSTOMERID>";
        RequestXML = RequestXML + "<DeliveryNo></DeliveryNo>";
        RequestXML = RequestXML + "<MultipleDeliveryNo>" + DeliveryNo + "</MultipleDeliveryNo>";
        RequestXML = RequestXML + "<Carrier>" + MyXml.GetElementsByTagName("Carrier")[0].InnerText + "</Carrier>";
        RequestXML = RequestXML + "<Service>" + MyXml.GetElementsByTagName("Service")[0].InnerText + "</Service>";
        RequestXML = RequestXML + "<AccountNumber>" + MyXml.GetElementsByTagName("AccountNumber")[0].InnerText.ToString() + "</AccountNumber>";
        RequestXML = RequestXML + "<PaymentType>" + MyXml.GetElementsByTagName("Payment")[0].InnerText + "</PaymentType>";
        RequestXML = RequestXML + "<UShipCode>" + MyXml.GetElementsByTagName("strCarrierCode")[0].InnerText + "</UShipCode>";
        RequestXML = RequestXML + "<PaymentType>" + MyXml.GetElementsByTagName("Payment")[0].InnerText + "</PaymentType>";
        RequestXML = RequestXML + "<ShipDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ShipDate>";
        RequestXML = RequestXML + "<PackageCount>" + MyXml.GetElementsByTagName("LPN").Count.ToString() + "</PackageCount>";
        RequestXML = RequestXML + "<PONumber>" + MyXml.GetElementsByTagName("PONo")[0].InnerText + "</PONumber>";
        RequestXML = RequestXML + "<InvoiceNumber>" + MyXml.GetElementsByTagName("InvoiceNo")[0].InnerText + "</InvoiceNumber>";
        RequestXML = RequestXML + "<ShipFromCountry></ShipFromCountry>";
        RequestXML = RequestXML + "<ShipToCountry></ShipToCountry>";
        RequestXML = RequestXML + "<PublishedFreight>" + dblTotalFreight.ToString() + "</PublishedFreight>";
        RequestXML = RequestXML + "<DiscountedFreight>" + discountTotalFreight.ToString() + "</DiscountedFreight>";
        RequestXML = RequestXML + "<ShippingNo></ShippingNo>";
        RequestXML = RequestXML + "<TotalWeight>" + MyXml.GetElementsByTagName("TOTALPACKWEIGHT")[0].InnerText.ToString() + "</TotalWeight>";
        RequestXML = RequestXML + "<MASTERTRACKINGNO>" + arrTrackingNumber[0].ToString() + "</MASTERTRACKINGNO>";

        try
        {
            int lblpackagecount = MyXml.GetElementsByTagName("LPN").Count;
            for (int j = 0; j < lblpackagecount; j++)
            {
                RequestXML = RequestXML + "<Pack>";
                RequestXML = RequestXML + "<Weight>" + MyXml.GetElementsByTagName("WEIGHT")[j].InnerText + "</Weight>";
                RequestXML = RequestXML + "<HandlingUnitId></HandlingUnitId>";
                RequestXML = RequestXML + "<TRACKINGNO>" + arrTrackingNumber[j].ToString() + "</TRACKINGNO>";
                try
                {
                    RequestXML = RequestXML + "<ReturnFreight>" + arrFreight[j].ToString() + "</ReturnFreight>";
                }
                catch (Exception)
                {
                }
                RequestXML = RequestXML + "<CartonID>" + MyXml.GetElementsByTagName("LPN_NUMBER")[j].InnerText + "</CartonID>";
                RequestXML = RequestXML + "</Pack>";
            }
        }
        catch (Exception)
        { }

        RequestXML = RequestXML + "</request>";
        return RequestXML;
    }
    public XmlDocument DoShipment(string ERPType, string DeliveryNo)
    {
        XmlDocument ShipmentError = new XmlDocument();
        XmlDocument ShipmentStatus = new XmlDocument();
        try
        {
            string DelReq = string.Empty;
            DelReq = PrepareGetDelxml(ERPType, DeliveryNo);

            string DelRes = string.Empty;
            zCustomsInfo _DelResObj = new zCustomsInfo();
            DelRes = _DelResObj.importgetdelivery(DelReq);

            XmlDocument ERPxml = new XmlDocument();
            ERPxml.LoadXml(DelRes);

            //ERPxml.Load(@"D:\Customers\Hitfar\ReadyXML.xml");

            if (ERPxml.GetElementsByTagName("Status")[0].InnerText.ToString().ToUpper() != "ERROR")
            {
                ShipmentStatus.LoadXml(ProcessShipment(ERPxml));
                try
                {
                    if (ShipmentStatus.GetElementsByTagName("Status")[0].InnerText.ToString() == "SUCCESS")
                    {
                        string strExportxml = PrepareGetExportxml(ShipmentStatus, DeliveryNo, ERPxml);
                        zCustomsInfo objexport = new zCustomsInfo();
                        objexport.exportgetdelivery(strExportxml);
                    }
                }
                catch (Exception)
                {

                }
                return ShipmentStatus;
            }
            else
            {
                ShipmentError.LoadXml(PrepareResponseXML(ERPxml.GetElementsByTagName("ERRORMESSAGE")[0].InnerText.ToString().ToUpper()));
                return ShipmentError;
            }
        }
        catch (Exception)
        {
            ShipmentError.LoadXml(PrepareResponseXML("Not well formed XML"));
            return ShipmentError;
        }
    }
    public string PrepareGetCancelExportxml(XmlDocument ShipmentRes, string DeliveryNo, XmlDocument MyXml)
    {
        string RequestXML = "";
        RequestXML = "<request>";
        RequestXML = RequestXML + "<Operation>Cancel</Operation>";
        RequestXML = RequestXML + "<PlantID>" + PlantID + "</PlantID>";
        RequestXML = RequestXML + "<CUSTOMERID>" + CUSTOMERID + "</CUSTOMERID>";
        RequestXML = RequestXML + "<ERPName>" + DefaltERP + "</ERPName>";
        RequestXML = RequestXML + "<DeliveryNo></DeliveryNo>";
        RequestXML = RequestXML + "<MultipleDeliveryNo>" + DeliveryNo + "</MultipleDeliveryNo>";
        RequestXML = RequestXML + "</request>";
        return RequestXML;
    }
    public XmlDocument VoidShipment(string ERPType, string DeliveryNo)
    {
        XmlDocument CancelStatus = new XmlDocument();
        XmlDocument CancelError = new XmlDocument();
        try
        {
            string CanRequestXML = string.Empty;
            CanRequestXML = PrepareGetDelCancelxml(ERPType, DeliveryNo);

            string DelCancelres = string.Empty;
            zCustomsInfo objcanSAPB1 = new zCustomsInfo();
            DelCancelres = objcanSAPB1.importgetdelivery(CanRequestXML);

            XmlDocument CanERPxml = new XmlDocument();
            CanERPxml.LoadXml(DelCancelres);

            if (CanERPxml.GetElementsByTagName("Status")[0].InnerText.ToString().ToUpper() != "ERROR")
            {
                CancelStatus.LoadXml(CancelShipment(CanERPxml));
                try
                {
                    if (CancelStatus.GetElementsByTagName("Status")[0].InnerText.ToString() == "SUCCESS")
                    {
                        string strExportxml = PrepareGetCancelExportxml(CancelStatus, DeliveryNo, CanERPxml);
                        objcanSAPB1.exportgetdelivery(strExportxml);
                    }
                }
                catch (Exception)
                {
                }
                return CancelStatus;
            }
            else
            {
                CancelError.LoadXml(PrepareResponseXML(CanERPxml.GetElementsByTagName("ERRORMESSAGE")[0].InnerText.ToString().ToUpper()));
                return CancelError;
            }
        }
        catch (Exception)
        {
            CancelError.LoadXml(PrepareResponseXML("Not well formed XML"));
            return CancelError;
        }
    }
    public string VoidShipment()
    {
        return "";
    }
    public string ProcessShipment(XmlDocument ERPxml)
    {
        try
        {
            string strReq = string.Empty;
            string dq = "\"";
            XmlNodeList ShipFromNodeList = ERPxml.SelectNodes("DeliveryInfo/ShipFromAddress");
            XmlNodeList ShipToNodeList = ERPxml.SelectNodes("DeliveryInfo/ShipToAddress");

            strReq = strReq + @"<?xml version=" + dq + "1.0" + dq + " encoding=" + dq + "utf-16" + dq + "?>" + "\n";
            strReq = strReq + "<soapenv:Envelope xmlns:soapenv=" + dq + "http://schemas.xmlsoap.org/soap/envelope/" + dq + " xmlns:proc=" + dq + "processRequest" + dq + ">" + "\n";
            strReq = strReq + "<soapenv:Header/>" + "\n";
            strReq = strReq + "<soapenv:Body>" + "\n";
            strReq = strReq + "<proc:processRequest>" + "\n";
            strReq = strReq + "<proc:strContents1>" + "\n";
            strReq = strReq + "<request>" + "\n";
            //-------------------------------------------Accounts Block-------------------------------------------------------------------//
            try
            {
                if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "UPS")
                {
                    strReq = strReq + "<Carrier>HAZUPSOPEN</Carrier>" + "\n";
                }
                else if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "FEDEX")
                {
                    strReq = strReq + "<Carrier>HAZFEDEXOPEN</Carrier>" + "\n";
                }
                else
                {
                    strReq = strReq + "<Carrier>" + ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() + "</Carrier>" + "\n";
                }

                if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATORESHIP" || ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATOR")
                {
                    strReq = strReq + "<UserID>" + ERPxml.GetElementsByTagName("ShippingKey")[0].InnerText.ToString() + "</UserID>" + "\n";
                }
                else
                {
                    strReq = strReq + "<UserID>" + ERPxml.GetElementsByTagName("UserID")[0].InnerText.ToString() + "</UserID>" + "\n";
                }

                strReq = strReq + "<Password>" + ERPxml.GetElementsByTagName("Password")[0].InnerText.ToString() + "</Password>" + "\n";
                strReq = strReq + "<AccountNumber>" + ERPxml.GetElementsByTagName("AccountNumber")[0].InnerText.ToString() + "</AccountNumber>" + "\n";
                strReq = strReq + "<MeterNumber>" + ERPxml.GetElementsByTagName("MeterNumber")[0].InnerText.ToString() + "</MeterNumber>" + "\n";
                strReq = strReq + "<CustomerTransactionId>" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "</CustomerTransactionId>" + "\n";
                strReq = strReq + "<ShipDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ShipDate>" + "\n";
                strReq = strReq + "<ServiceType>" + ERPxml.GetElementsByTagName("Service")[0].InnerText.ToString() + "</ServiceType>" + "\n";
                strReq = strReq + "<SpecialInstructions/>" + "\n";
                strReq = strReq + "<PWC/>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Accounts Information");
            }
            //-----------------------------------------------Sender Block---------------------------------------------------------------//

            try
            {
                strReq = strReq + "<Sender>" + "\n";
                strReq = strReq + "<CompanyName>" + ShipFromNodeList[0].SelectSingleNode("sPlantCompany").InnerText.ToString() + "</CompanyName>" + "\n";
                strReq = strReq + "<Contact>" + ShipFromNodeList[0].SelectSingleNode("sPlantContact").InnerText.ToString() + "</Contact>" + "\n";
                strReq = strReq + "<StreetLine1>" + ShipFromNodeList[0].SelectSingleNode("sPlantAddressLine1").InnerText.ToString() + "</StreetLine1>" + "\n";
                strReq = strReq + "<StreetLine2>" + ShipFromNodeList[0].SelectSingleNode("sPlantAddressLine2").InnerText.ToString() + "</StreetLine2>" + "\n";
                strReq = strReq + "<StreetLine3/>" + "\n";
                strReq = strReq + "<City>" + ShipFromNodeList[0].SelectSingleNode("sPlantCity").InnerText.ToString() + "</City>" + "\n";
                strReq = strReq + "<StateOrProvinceCode>" + ShipFromNodeList[0].SelectSingleNode("sPlantState").InnerText.ToString() + "</StateOrProvinceCode>" + "\n";
                strReq = strReq + "<PostalCode>" + ShipFromNodeList[0].SelectSingleNode("sPlantZipCode").InnerText.ToString() + "</PostalCode>" + "\n";
                strReq = strReq + "<CountryCode>" + ShipFromNodeList[0].SelectSingleNode("sPlantCountry").InnerText.ToString() + "</CountryCode>" + "\n";
                strReq = strReq + "<Phone>" + ShipFromNodeList[0].SelectSingleNode("sPlantPhone").InnerText.ToString() + "</Phone>" + "\n";
                strReq = strReq + "<Email>" + ShipFromNodeList[0].SelectSingleNode("sPlantEmail").InnerText.ToString() + "</Email>" + "\n";
                strReq = strReq + "</Sender>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Sender Information");
            }
            //---------------------------------------------Receiver Block--------------------------------------------------//
            try
            {
                strReq = strReq + "<Recipient>" + "\n";
                strReq = strReq + "<CompanyName>" + ShipToNodeList[0].SelectSingleNode("Company").InnerText.ToString() + "</CompanyName>" + "\n";
                strReq = strReq + "<Contact>" + ShipToNodeList[0].SelectSingleNode("Contact").InnerText.ToString() + "</Contact>" + "\n";
                strReq = strReq + "<StreetLine1>" + ShipToNodeList[0].SelectSingleNode("AddressLine1").InnerText.ToString() + "</StreetLine1>" + "\n";
                strReq = strReq + "<StreetLine2>" + ShipToNodeList[0].SelectSingleNode("AddressLine2").InnerText.ToString() + "</StreetLine2>" + "\n";
                strReq = strReq + "<StreetLine3/>" + "\n";
                strReq = strReq + "<City>" + ShipToNodeList[0].SelectSingleNode("City").InnerText.ToString() + "</City>" + "\n";
                strReq = strReq + "<StateOrProvinceCode>" + ShipToNodeList[0].SelectSingleNode("State").InnerText.ToString() + "</StateOrProvinceCode>" + "\n";
                strReq = strReq + "<PostalCode>" + ShipToNodeList[0].SelectSingleNode("ZipCode").InnerText.ToString() + "</PostalCode>" + "\n";
                strReq = strReq + "<CountryCode>" + ShipToNodeList[0].SelectSingleNode("Country").InnerText.ToString() + "</CountryCode>" + "\n";
                strReq = strReq + "<Phone>" + ShipToNodeList[0].SelectSingleNode("Phone").InnerText.ToString() + "</Phone>" + "\n";
                strReq = strReq + "<Email>" + ShipToNodeList[0].SelectSingleNode("Email").InnerText.ToString() + "</Email>" + "\n";
                strReq = strReq + "</Recipient>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Receiver Information");
            }
            //--------------------------------------Payment Info Block-------------------------------------------------//
            try
            {
                strReq = strReq + "<Paymentinformation>" + "\n";

                if(ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() == "SENDER")
                {
                    strReq = strReq + "<PaymentType>" + ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() + "</PaymentType>" + "\n";
                }
                else if(ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() == "THIRD PARTY" || ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() == "THIRD_PARTY")
                {
                    strReq = strReq + "<PaymentType>THIRD_PARTY</PaymentType>" + "\n";
                }
                else
                {
                    strReq = strReq + "<PaymentType>" + ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() + "</PaymentType>" + "\n";
                }
                
                if (ERPxml.GetElementsByTagName("Payment")[0].InnerText.ToString().ToUpper() == "SENDER")
                {
                    if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "CANADAPOST")
                    {
                        if (ERPxml.GetElementsByTagName("CanadaPostPayorAccountNumber")[0].InnerText.ToString() != "")
                        {
                            strReq = strReq + "<PayerAccountNumber>" + ERPxml.GetElementsByTagName("CanadaPostPayorAccountNumber")[0].InnerText.ToString() + "</PayerAccountNumber>" + "\n";
                        }
                        else
                        {
                            strReq = strReq + "<PayerAccountNumber>" + ERPxml.GetElementsByTagName("AccountNumber")[0].InnerText.ToString() + "</PayerAccountNumber>" + "\n";
                        }
                    }
                    else
                    {
                        strReq = strReq + "<PayerAccountNumber>" + ERPxml.GetElementsByTagName("AccountNumber")[0].InnerText.ToString() + "</PayerAccountNumber>" + "\n";
                    }
                }
                else
                {
                    strReq = strReq + "<PayerAccountNumber>" + ERPxml.GetElementsByTagName("PaymentAccount")[0].InnerText.ToString() + "</PayerAccountNumber>" + "\n";
                }
                strReq = strReq + "<PayerCountryCode>" + ERPxml.GetElementsByTagName("PaymentCountry")[0].InnerText.ToString() + "</PayerCountryCode>" + "\n";
                strReq = strReq + "<PayerAccountZipCode>" + ERPxml.GetElementsByTagName("PaymentZipCode")[0].InnerText.ToString() + "</PayerAccountZipCode>" + "\n";
                strReq = strReq + "</Paymentinformation>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Payment Information");
            }
            //------------------------------------------Package Info Block--------------------------------------------------------------------//
            try
            {
                strReq = strReq + "<PackageCount>" + ERPxml.GetElementsByTagName("LPN").Count.ToString() + "</PackageCount>" + "\n";
                strReq = strReq + "<TotalWeight>" + ERPxml.GetElementsByTagName("TOTALPACKWEIGHT")[0].InnerText.ToString() + "</TotalWeight>" + "\n";

                for (int p = 0; p <= ERPxml.GetElementsByTagName("LPN").Count - 1; p++)
                {
                    strReq = strReq + "<Packagedetails>" + "\n";
                    strReq = strReq + "<WeightValue>" + ERPxml.GetElementsByTagName("WEIGHT")[p].InnerText.ToString() + "</WeightValue>" + "\n";
                    strReq = strReq + "<WeightUnits>" + ShipFromNodeList[0].SelectSingleNode("sPlantWeightUnits").InnerText.ToString() + "</WeightUnits>" + "\n";
                    try
                    {
                        if (ERPxml.GetElementsByTagName("Length")[0].InnerText.ToString() != "" && ERPxml.GetElementsByTagName("Width")[0].InnerText.ToString() != "" && ERPxml.GetElementsByTagName("Height")[0].InnerText.ToString() != "")
                        {
                            string Length = string.Empty;
                            string Width = string.Empty;
                            string Height = string.Empty;

                            Length = ERPxml.GetElementsByTagName("Length")[0].InnerText.ToString();
                            Width = ERPxml.GetElementsByTagName("Width")[0].InnerText.ToString();
                            Height = ERPxml.GetElementsByTagName("Height")[0].InnerText.ToString();

                            strReq = strReq + "<Length>" + Length.Substring(0, Length.IndexOf(".")) + "</Length>" + "\n";
                            strReq = strReq + "<Width>" + Width.Substring(0, Width.IndexOf(".")) + "</Width>" + "\n";
                            strReq = strReq + "<Height>" + Height.Substring(0, Height.IndexOf(".")) + "</Height>" + "\n";
                        }
                    }
                    catch (Exception)
                    { }

                    if (ShipFromNodeList[0].SelectSingleNode("sPlantDimUnits").InnerText.ToString() == "IN")
                    {
                        strReq = strReq + "<DimensionUnit>IN</DimensionUnit>" + "\n";
                    }
                    else
                    {
                        strReq = strReq + "<DimensionUnit>CM</DimensionUnit>" + "\n";
                    }

                    strReq = strReq + "<CODAmount/>" + "\n";
                    strReq = strReq + "<CODCurrencyCode/>" + "\n";
                    strReq = strReq + "<InsuranceAmount/>" + "\n";
                    strReq = strReq + "<InsuranceCurrencyCode/>" + "\n";

                    if (ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() != "")
                    {
                        if (ERPxml.GetElementsByTagName("BBY")[0].InnerText.ToString() == "TRUE" && ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() != "PUROLATORESHIP")
                        {
                            strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                            strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                            strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                        }
                        else if(ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "CANADAPOST")
                        {
                            strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                            strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                            strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                        }
                        else if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATORESHIP" && ERPxml.GetElementsByTagName("BBY")[0].InnerText.ToString() == "TRUE")
                        {
                            strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                            strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                            strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                        }
                        else
                        {
                            strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                            strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                            strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                        }                        
                    }
                    else
                    {
                        strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                        strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                        strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                    }
                    strReq = strReq + "</Packagedetails>" + "\n";
                }
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Package Information");
            }
            //------------------------------------Reference Block--------------------------------------------------------//
            try
            {
                strReq = strReq + "<Referencedetails>" + "\n";
                if (ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() != "")
                {
                    if (ERPxml.GetElementsByTagName("BBY")[0].InnerText.ToString() == "TRUE" && ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() != "PUROLATORESHIP")
                    {
                        strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                        strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                        strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                    }
                    else if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "CANADAPOST")
                    {
                        strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                        strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                        strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                    }
                    else if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATORESHIP" && ERPxml.GetElementsByTagName("BBY")[0].InnerText.ToString() == "TRUE")
                    {
                        strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                        strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                        strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                    }
                    else
                    {
                        strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                        strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                        strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                    }
                }
                else
                {
                    strReq = strReq + "<CUSTOMERREFERENCE>" + ERPxml.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + "</CUSTOMERREFERENCE>" + "\n";
                    strReq = strReq + "<INVOICENUMBER>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</INVOICENUMBER>" + "\n";
                    strReq = strReq + "<PONUMBER>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PONUMBER>" + "\n";
                }
                
                strReq = strReq + "</Referencedetails>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Reference Information");
            }
            //---------------------------------------Special Services Block---------------------------------------------------------//
            try
            {
                strReq = strReq + "<SpecialServices>" + "\n";

                if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "CANADAPOST")
                {
                    strReq = strReq + "<DNS>TRUE</DNS>" + "\n";
                }

                strReq = strReq + "<ReturnServiceCode/>" + "\n";
                strReq = strReq + "<SaturdayDelivery/>" + "\n";
                strReq = strReq + "<SaturdayPickup/>" + "\n";
                strReq = strReq + "<InsidePickup/>" + "\n";
                strReq = strReq + "<InsideDelivery/>" + "\n";
                
                if (ERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATORESHIP")
                {
                    strReq = strReq + "<SignatureRequired></SignatureRequired>" + "\n";
                }
                else
                {
                    strReq = strReq + "<SignatureRequired/>" + "\n";
                }

                strReq = strReq + "</SpecialServices>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of SpecialServices Information");
            }
            //-------------------------------------International Block-------------------------------------------------//
            try
            {
                if (ShipFromNodeList[0].SelectSingleNode("sPlantCountry").InnerText.ToString() != ShipToNodeList[0].SelectSingleNode("Country").InnerText.ToString())
                {

                    strReq = strReq + "<InternationalDetail>" + "\n";
                    for (int It = 0; It <= ERPxml.GetElementsByTagName("ItemNo").Count - 1; It++)
                    {
                        strReq = strReq + "<Commodities>" + "\n";
                        strReq = strReq + "<Description>" + ERPxml.GetElementsByTagName("ItemDescription").ToString() + "</Description>" + "\n";
                        strReq = strReq + "<Quantity>" + ERPxml.GetElementsByTagName("ItemQuantity").ToString() + "</Quantity>" + "\n";
                        strReq = strReq + "<CountryOfManufacture>" + ShipFromNodeList[0].SelectSingleNode("sPlantCountry").InnerText.ToString() + "</CountryOfManufacture>" + "\n";
                        strReq = strReq + "<UnitPrice>1.00</UnitPrice>" + "\n";
                        strReq = strReq + "<HarmonizedCode>8544429000</HarmonizedCode>" + "\n";
                        strReq = strReq + "<PartNumber/>" + "\n";
                        strReq = strReq + "</Commodities>" + "\n";
                    }
                    strReq = strReq + "</InternationalDetail>" + "\n";
                }
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of International Information");
            }
            //---------------------------------------------Duties Block----------------------------------------------------//
            try
            {
                strReq = strReq + "<DeclaredValue></DeclaredValue>" + "\n";
                strReq = strReq + "<DeclaredValueCurrency></DeclaredValueCurrency>" + "\n";
                strReq = strReq + "<InvoiceNumber>" + ERPxml.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString() + "</InvoiceNumber>" + "\n";
                strReq = strReq + "<InvoiceDate></InvoiceDate>" + "\n";
                strReq = strReq + "<PurchaseOrderNumber>" + ERPxml.GetElementsByTagName("PONo")[0].InnerText.ToString() + "</PurchaseOrderNumber>" + "\n";
                strReq = strReq + "<ReasonForExport>SOLD</ReasonForExport>" + "\n";
                strReq = strReq + "<CurrencyCode>CAD</CurrencyCode>" + "\n";
                strReq = strReq + "<DutiesPaymentType>RECIPIENT</DutiesPaymentType>" + "\n";
            }
            catch (Exception)
            {
                return PrepareResponseXML("Missing of Duties Information");
            }
            //--------------------------------------------------------------------------------------------------------------//
            strReq = strReq + "</request>" + "\n";
            strReq = strReq + "</proc:strContents1>" + "\n";
            strReq = strReq + "<proc:tname>ECSSHIP_XCARRIER_Prod.xml</proc:tname>" + "\n";
            strReq = strReq + "<proc:isEncript></proc:isEncript>" + "\n";
            strReq = strReq + "<proc:opt1>xmlns=\"processRequest\"</proc:opt1>" + "\n";
            strReq = strReq + "<proc:opt2></proc:opt2>" + "\n";
            strReq = strReq + "<proc:opt4></proc:opt4>" + "\n";
            strReq = strReq + "<proc:opt5></proc:opt5>" + "\n";
            strReq = strReq + "<proc:opt6></proc:opt6>" + "\n";
            strReq = strReq + "<proc:opt7></proc:opt7>" + "\n";
            strReq = strReq + "<proc:opt8></proc:opt8>" + "\n";
            strReq = strReq + "<proc:opt9></proc:opt9>" + "\n";
            strReq = strReq + "<proc:opt10>Y</proc:opt10>" + "\n";
            strReq = strReq + "</proc:processRequest>" + "\n";
            strReq = strReq + "</soapenv:Body>" + "\n";
            strReq = strReq + "</soapenv:Envelope>";

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(strReq);

            HttpWebRequest myRequest = default(HttpWebRequest);

            myRequest = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings.Get("ServiceEngine").ToString() + "/Service.asmx");
            myRequest.AllowAutoRedirect = false;

            myRequest.Method = "POST";
            myRequest.Headers.Add("SOAPAction", "processRequest/processRequest");
            myRequest.ContentType = "text/xml;charset=UTF-8";
            //strxml = result;
            Stream RequestStream = myRequest.GetRequestStream();
            try
            {
                string mydocpath1 = "C:\\Processweaver\\BackUp";

                using (System.IO.StreamWriter outfile =
                  new System.IO.StreamWriter(mydocpath1 + @"\ECSAPIREQ_" + ERPxml.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + ".xml"))
                {
                    outfile.Write(strReq);
                }
            }
            catch (Exception)
            {
            }

            byte[] SomeBytes = Encoding.UTF8.GetBytes(strReq);
            RequestStream.Write(SomeBytes, 0, SomeBytes.Length);
            RequestStream.Close();

            System.Xml.XmlDocument MyXMLrep = default(System.Xml.XmlDocument);
            MyXMLrep = new System.Xml.XmlDocument();
            System.Xml.XmlDocument MyXMLDocument;
            MyXMLDocument = new System.Xml.XmlDocument();

            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Get the stream. 
                    Stream ReceiveStreamFDX = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    //send the stream to a reader. 
                    StreamReader readStream = new StreamReader(ReceiveStreamFDX, encode);
                    //Read the result 
                    String Result = readStream.ReadToEnd();
                    MyXMLrep.LoadXml(Result);

                    Result = Result.Replace("soap:", "");
                    Result = Result.Replace("<processRequestResponse xmlns=\"processRequest\">", "<processRequestResponse>");
                    Result = Result.Replace("<FedExReply xmlns=\"\">", "<FedExReply>");
                    Result = Result.Replace("xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
                    Result = Result.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                    Result = Result.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
                    MyXMLDocument.LoadXml(Result);
                    //MyXMLDocument.Load(@"C:\Processweaver\BackUp\ECSAPIRES_006275400008015992.xml");

                    try
                    {
                        string mydocpath1 = "C:\\Processweaver\\BackUp";
                        using (System.IO.StreamWriter outfile =
                          new System.IO.StreamWriter(mydocpath1 + @"\ECSAPIRES_" + ERPxml.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + ".xml"))
                        {
                            outfile.Write(Result);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    XmlNodeList Successnodelist;
                    Successnodelist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Response");
                    try
                    {
                        if (Successnodelist.Count > 0)
                        {
                            XmlNodeList Tracklist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Response/Package/TrackingNumber");
                            if (Tracklist.Count > 0)
                            {
                                arrTrackingNumber = new String[Tracklist.Count];
                                for (int iTrack = 0; iTrack <= Tracklist.Count - 1; iTrack++)
                                {
                                    arrTrackingNumber[iTrack] = Tracklist[iTrack].InnerText.ToString();
                                }
                            }
                            XmlNodeList Freightlist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Response/Package/Freight");
                            if (Freightlist.Count > 0)
                            {
                                arrFreight = new String[Freightlist.Count];
                                for (int iFreight = 0; iFreight <= Freightlist.Count - 1; iFreight++)
                                {
                                    arrFreight[iFreight] = Freightlist[iFreight].InnerText;
                                }
                            }
                            XmlNodeList DiscountFieightList = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Response/Package/DiscountedFreight");
                            if (DiscountFieightList.Count > 0)
                            {
                                arrDiscountFreight = new String[DiscountFieightList.Count];
                                for (int iFreight = 0; iFreight <= DiscountFieightList.Count - 1; iFreight++)
                                {
                                    arrDiscountFreight[iFreight] = DiscountFieightList[iFreight].InnerText;
                                }
                            }
                            try
                            {
                                dblTotalFreight = Convert.ToDouble(MyXMLDocument.GetElementsByTagName("TotalFreight")[0].InnerText);
                                discountTotalFreight = Convert.ToDouble(MyXMLDocument.GetElementsByTagName("TotalDiscountedFreight")[0].InnerText);
                            }
                            catch (Exception)
                            {
                                string totFreight = string.Empty;
                                string disFreight = string.Empty;
                                totFreight = MyXMLDocument.GetElementsByTagName("TotalFreight")[0].InnerText;
                                disFreight = MyXMLDocument.GetElementsByTagName("TotalDiscountedFreight")[0].InnerText;

                                try
                                {
                                    dblTotalFreight = Convert.ToDouble(totFreight.Replace("CAD", ""));
                                    discountTotalFreight = Convert.ToDouble(disFreight.Replace("CAD", ""));
                                }
                                catch (Exception)
                                {
                                    dblTotalFreight = 0.00;
                                    discountTotalFreight = 0.00;
                                }
                            }
                            //--- xCarrier Update----------//
                            XCARRIERMVC_SAVE(MyXMLDocument, ERPxml);
                            //----------------------------//
                            return PrepareSuccessResponseXML(Successnodelist[0].InnerXml.ToString());
                        }
                        else
                        {
                            XmlNodeList resnodeList;
                            resnodeList = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult");
                            return PrepareResponseXML(resnodeList[0].InnerXml.ToString());
                        }
                    }
                    catch (Exception)
                    {
                        XmlNodeList errornodelist;
                        try
                        {
                            errornodelist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Response/Error");
                            if (errornodelist.Count < 0 || errornodelist.Count == 0)
                            {
                                errornodelist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/Error");
                            }
                            if (errornodelist.Count < 0 || errornodelist.Count == 0)
                            {
                                errornodelist = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult/FedExReply/Error");
                            }
                            return PrepareResponseXML(errornodelist[0].InnerXml.ToString());
                        }
                        catch (Exception)
                        {
                            XmlNodeList resnodeList;
                            resnodeList = MyXMLDocument.SelectNodes("Envelope/Body/processRequestResponse/processRequestResult");
                            return PrepareResponseXML(resnodeList[0].InnerXml.ToString());
                        }
                    }
                }
                else
                {
                    return PrepareResponseXML("Carrier Web Service Connection Error ...!");
                }
            }
            catch (Exception Ex)
            {
                return PrepareResponseXML(Ex.Message.ToString());
            }
        }
        catch (Exception Ex)
        {
            return PrepareResponseXML(Ex.Message.ToString());
        }
    }
    public string CancelShipment(XmlDocument CanERPxml)
    {
        try
        {
            string strReq = string.Empty;
            string dq = "\"";

            strReq = strReq + @"<?xml version=" + dq + "1.0" + dq + " encoding=" + dq + "utf-16" + dq + "?>" + "\n";
            strReq = strReq + "<soapenv:Envelope xmlns:soapenv=" + dq + "http://schemas.xmlsoap.org/soap/envelope/" + dq + " xmlns:proc=" + dq + "processRequest" + dq + ">" + "\n";
            strReq = strReq + "<soapenv:Header/>" + "\n";
            strReq = strReq + "<soapenv:Body>" + "\n";
            strReq = strReq + "<proc:processRequest>" + "\n";
            strReq = strReq + "<proc:strContents1>" + "\n";
            strReq = strReq + "<request>" + "\n";

            //--------------------------------------------------------------------------------------------------------------//
            strReq = strReq + "<Carrier>" + CanERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() + "</Carrier>" + "\n";
            if (CanERPxml.GetElementsByTagName("Carrier")[0].InnerText.ToString().ToUpper() == "PUROLATOR")
            {
                strReq = strReq + "<UserID>" + CanERPxml.GetElementsByTagName("ShippingKey")[0].InnerText.ToString() + "</UserID>" + "\n";
            }
            else
            {
                strReq = strReq + "<UserID>" + CanERPxml.GetElementsByTagName("UserID")[0].InnerText.ToString() + "</UserID>" + "\n";
            }
            strReq = strReq + "<Password>" + CanERPxml.GetElementsByTagName("Password")[0].InnerText.ToString() + "</Password>" + "\n";
            strReq = strReq + "<AccountNumber>" + CanERPxml.GetElementsByTagName("AccountNumber")[0].InnerText.ToString() + "</AccountNumber>" + "\n";
            strReq = strReq + "<MeterNumber>" + CanERPxml.GetElementsByTagName("MeterNumber")[0].InnerText.ToString() + "</MeterNumber>" + "\n";
            strReq = strReq + "<ShipDate>" + DateTime.Now.ToString("yyyy-MM-dd") + "</ShipDate>" + "\n";
            for (int q = 0; q <= CanERPxml.GetElementsByTagName("Pack").Count - 1; q++)
            {
                strReq = strReq + "<Package>" + "\n";
                strReq = strReq + "<TrackingNumber>" + CanERPxml.GetElementsByTagName("TrackingNo")[q].InnerText.ToString().ToUpper() + "</TrackingNumber>" + "\n";
                strReq = strReq + "</Package>" + "\n";
            }
            //--------------------------------------------------------------------------------------------------------------//
            strReq = strReq + "</request>" + "\n";
            strReq = strReq + "</proc:strContents1>" + "\n";
            strReq = strReq + "<proc:tname>ECSVOID_PROD</proc:tname>" + "\n";
            strReq = strReq + "<proc:isEncript></proc:isEncript>" + "\n";
            strReq = strReq + "<proc:opt1>xmlns=\"processRequest\"</proc:opt1>" + "\n";
            strReq = strReq + "<proc:opt2></proc:opt2>" + "\n";
            strReq = strReq + "<proc:opt4></proc:opt4>" + "\n";
            strReq = strReq + "<proc:opt5></proc:opt5>" + "\n";
            strReq = strReq + "<proc:opt6></proc:opt6>" + "\n";
            strReq = strReq + "<proc:opt7></proc:opt7>" + "\n";
            strReq = strReq + "<proc:opt8></proc:opt8>" + "\n";
            strReq = strReq + "<proc:opt9></proc:opt9>" + "\n";
            strReq = strReq + "<proc:opt10>Y</proc:opt10>" + "\n";
            strReq = strReq + "</proc:processRequest>" + "\n";
            strReq = strReq + "</soapenv:Body>" + "\n";
            strReq = strReq + "</soapenv:Envelope>";

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(strReq);

            HttpWebRequest myRequest = default(HttpWebRequest);

            myRequest = (HttpWebRequest)HttpWebRequest.Create(ConfigurationManager.AppSettings.Get("ServiceEngine").ToString() + "/Service.asmx");
            myRequest.AllowAutoRedirect = false;

            myRequest.Method = "POST";
            myRequest.Headers.Add("SOAPAction", "processRequest/processRequest");
            myRequest.ContentType = "text/xml;charset=UTF-8";
            //strxml = result;
            Stream RequestStream = myRequest.GetRequestStream();
            try
            {
                string mydocpath1 = "C:\\Processweaver\\BackUp";

                using (System.IO.StreamWriter outfile =
                  new System.IO.StreamWriter(mydocpath1 + @"\ECSAPIREQ_CANCEL.xml"))
                {
                    outfile.Write(strReq);
                }
            }
            catch (Exception)
            {
            }

            byte[] SomeBytes = Encoding.UTF8.GetBytes(strReq);
            RequestStream.Write(SomeBytes, 0, SomeBytes.Length);
            RequestStream.Close();

            System.Xml.XmlDocument MyXMLrep = default(System.Xml.XmlDocument);
            MyXMLrep = new System.Xml.XmlDocument();
            System.Xml.XmlDocument MyXMLCanDocument;
            MyXMLCanDocument = new System.Xml.XmlDocument();
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    //Get the stream. 
                    Stream ReceiveStreamFDX = myResponse.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    //send the stream to a reader. 
                    StreamReader readStream = new StreamReader(ReceiveStreamFDX, encode);
                    //Read the result 
                    String Result = readStream.ReadToEnd();
                    MyXMLrep.LoadXml(Result);

                    Result = Result.Replace("soap:", "");
                    Result = Result.Replace("<processRequestResponse xmlns=\"processRequest\">", "<processRequestResponse>");
                    Result = Result.Replace("<FedExReply xmlns=\"\">", "<FedExReply>");
                    MyXMLCanDocument.LoadXml(Result);

                    try
                    {
                        string mydocpath1 = "C:\\Processweaver\\BackUp";
                        using (System.IO.StreamWriter outfile =
                          new System.IO.StreamWriter(mydocpath1 + @"\ECSAPIRES_CANCEL.xml"))
                        {
                            outfile.Write(Result);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    if (MyXMLCanDocument.GetElementsByTagName("StatusMessage")[0].InnerText.ToString().ToUpper() != "SUCCESS")
                    {
                        return PrepareResponseXML(MyXMLCanDocument.GetElementsByTagName("StatusMessage")[0].InnerText.ToString().ToUpper());
                    }
                    else
                    {
                        return PrepareSuccessResponseXML(MyXMLCanDocument.GetElementsByTagName("StatusMessage")[0].InnerText.ToString().ToUpper());
                    }
                }
                else
                {
                    return PrepareResponseXML("Carrier Web Service Connection Error ...!");
                }
            }
            catch (Exception Ex)
            {
                return PrepareResponseXML(Ex.Message.ToString());
            }
        }
        catch (Exception Ex)
        {
            return PrepareResponseXML(Ex.Message.ToString());
        }
    }
    public string PrepareResponseXML(string Info)
    {
        string strResponse = "";
        try
        {
            strResponse = "<Result>" + System.Environment.NewLine;
            strResponse = strResponse + "<Status>ERROR</Status>" + System.Environment.NewLine;
            strResponse = strResponse + "<Message>" + System.Environment.NewLine;
            strResponse = strResponse + Info + System.Environment.NewLine;
            strResponse = strResponse + "</Message>" + System.Environment.NewLine;
            strResponse = strResponse + "</Result>" + System.Environment.NewLine;
        }
        catch (Exception ex)
        {
            strResponse = strResponse + ex.Message + "</Message>" + System.Environment.NewLine;
            strResponse = strResponse + "</Result>" + System.Environment.NewLine;
        }
        return strResponse;
    }
    public string PrepareSuccessResponseXML(string Info)
    {
        string strResponse = "";
        try
        {
            strResponse = "<Result>" + System.Environment.NewLine;
            strResponse = strResponse + "<Status>SUCCESS</Status>" + System.Environment.NewLine;
            strResponse = strResponse + "<DATA>" + Info + "</DATA>" + System.Environment.NewLine;
            strResponse = strResponse + "</Result>" + System.Environment.NewLine;
        }
        catch (Exception)
        {
        }
        return strResponse;
    }
    public string XCARRIERMVC_SAVE(XmlDocument XMLShipRes, XmlDocument XMLShipReq)
    {
        string strXML = string.Empty;
        string strResponse = string.Empty;
        string URL = string.Empty;
        string MasterTrack = string.Empty;

        try
        {
            XmlNodeList ShipFromList = XMLShipReq.SelectNodes("DeliveryInfo/ShipFromAddress");
            XmlNodeList ShipToList = XMLShipReq.SelectNodes("DeliveryInfo/ShipToAddress");
            XmlNodeList Packages = XMLShipReq.SelectNodes("DeliveryInfo/PACKAGEINFO/LPN");
            string Forms = XMLShipRes.GetElementsByTagName("XFORMSANDLABELS")[0].OuterXml.ToString();

            strXML = strXML + "<HEADER>" + System.Environment.NewLine;
            try
            {
                strXML = strXML + "<SHIPPINGPOINT>" + XMLShipReq.GetElementsByTagName("PLANTID")[0].InnerText.ToString() + "</SHIPPINGPOINT>" + System.Environment.NewLine;
            }
            catch (Exception)
            {
                strXML = strXML + "<SHIPPINGPOINT>1</SHIPPINGPOINT>" + System.Environment.NewLine;
            }
            strXML = strXML + "<SHIPDATE>" + DateTime.Now.ToString("yyyy-MM-dd") + "</SHIPDATE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTIME>" + DateTime.Now.ToString("HH:mm:ss") + "</SHIPTIME>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPMENTNUMBER>" + XMLShipReq.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + "</SHIPMENTNUMBER>" + System.Environment.NewLine;
            strXML = strXML + "<DELIVERY>" + XMLShipReq.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString() + "</DELIVERY>" + System.Environment.NewLine;
            try
            {
                strXML = strXML + "<CARRIERTYPE>" + XMLShipReq.GetElementsByTagName("CarrierName")[0].InnerText.ToString() + "</CARRIERTYPE>" + System.Environment.NewLine;
            }
            catch (Exception)
            {
                strXML = strXML + "<CARRIERTYPE>" + XMLShipReq.GetElementsByTagName("Carrier")[0].InnerText.ToString() + "</CARRIERTYPE>" + System.Environment.NewLine;
            }
            
            strXML = strXML + "<CARRIERSERVICE>" + XMLShipReq.GetElementsByTagName("ServiceName")[0].InnerText.ToString() + "</CARRIERSERVICE>" + System.Environment.NewLine;

            try
            {
                try
                {
                    if (XMLShipRes.GetElementsByTagName("MasterTracking")[0].InnerText.ToString() != string.Empty)
                    {
                        MasterTrack = XMLShipRes.GetElementsByTagName("MasterTracking")[0].InnerText.ToString();
                    }
                }
                catch(Exception)
                {

                }
                try
                {
                    if (XMLShipRes.GetElementsByTagName("SIN")[0].InnerText.ToString() != string.Empty)
                    {
                        MasterTrack = XMLShipRes.GetElementsByTagName("SIN")[0].InnerText.ToString();
                    }
                }
                catch (Exception)
                {

                }
            }
            catch(Exception)
            { }
            strXML = strXML + "<MASTERTRACKING>" + MasterTrack + "</MASTERTRACKING>" + System.Environment.NewLine;
            strXML = strXML + "<TOTALNUMBEROFPACKAGES>" + XMLShipReq.GetElementsByTagName("LPN").Count.ToString() + "</TOTALNUMBEROFPACKAGES>" + System.Environment.NewLine;
            strXML = strXML + "<TOTALWEIGHT>" + XMLShipReq.GetElementsByTagName("TOTALPACKWEIGHT")[0].InnerText.ToString() + "</TOTALWEIGHT>" + System.Environment.NewLine;
            strXML = strXML + "<LISTFREIGHT>" + XMLShipRes.GetElementsByTagName("TotalFreight")[0].InnerText.ToString() + "</LISTFREIGHT>" + System.Environment.NewLine;
            strXML = strXML + "<DISCOUNTFREIGHT>" + XMLShipRes.GetElementsByTagName("TotalDiscountedFreight")[0].InnerText.ToString() + "</DISCOUNTFREIGHT>" + System.Environment.NewLine;
            strXML = strXML + "<CURRENCY>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("sPlantCurrency")[0].InnerText.ToString()) + "</CURRENCY>" + System.Environment.NewLine;
            strXML = strXML + "<PAYMENTCODE>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("Payment")[0].InnerText.ToString()) + "</PAYMENTCODE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPPERACCOUNT>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("AccountNumber")[0].InnerText.ToString()) + "</SHIPPERACCOUNT>" + System.Environment.NewLine;
            strXML = strXML + "<PAYORACCOUNT>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("PaymentAccount")[0].InnerText.ToString()) + "</PAYORACCOUNT>" + System.Environment.NewLine;
            strXML = strXML + "<REFERENCE1>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("PONo")[0].InnerText.ToString()) + "</REFERENCE1>" + System.Environment.NewLine;
            strXML = strXML + "<REFERENCE2>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString()) + "</REFERENCE2>" + System.Environment.NewLine;
            strXML = strXML + "<SALESORDER>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("InvoiceNo")[0].InnerText.ToString()) + "</SALESORDER>" + System.Environment.NewLine;
            strXML = strXML + "<CUSTOMERPO>" + removeSplcharactersXML(XMLShipReq.GetElementsByTagName("PONo")[0].InnerText.ToString()) + "</CUSTOMERPO>" + System.Environment.NewLine;
            strXML = strXML + "<COSTOBJECT></COSTOBJECT>" + System.Environment.NewLine;
            try
            {
                strXML = strXML + "<COSTOBJECTVALUE>" + XMLShipReq.GetElementsByTagName("CUSTOMERID")[0].InnerText.ToString() + "</COSTOBJECTVALUE>" + System.Environment.NewLine;
            }
            catch (Exception)
            {
                strXML = strXML + "<COSTOBJECTVALUE></COSTOBJECTVALUE>" + System.Environment.NewLine;
            }
            strXML = strXML + "<CHARGEABLEWEIGHT>" + XMLShipReq.GetElementsByTagName("TOTALPACKWEIGHT")[0].InnerText.ToString() + "</CHARGEABLEWEIGHT>" + System.Environment.NewLine;

            strXML = strXML + "<SHIPTOCOMPANY>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("Company").InnerText.ToString()) + "</SHIPTOCOMPANY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOCONTACT>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("Contact").InnerText.ToString()) + "</SHIPTOCONTACT>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOADDRESS1>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("AddressLine1").InnerText.ToString()) + "</SHIPTOADDRESS1>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOADDRESS2>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("AddressLine2").InnerText.ToString()) + "</SHIPTOADDRESS2>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOADDRESS3></SHIPTOADDRESS3>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOCITY>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("City").InnerText.ToString()) + "</SHIPTOCITY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOSTATE>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("State").InnerText.ToString()) + "</SHIPTOSTATE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOCOUNTRY>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("Country").InnerText.ToString()) + "</SHIPTOCOUNTRY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOZIP>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("ZipCode").InnerText.ToString()) + "</SHIPTOZIP>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOPHONE>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("Phone").InnerText.ToString()) + "</SHIPTOPHONE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPTOEMAIL>" + removeSplcharactersXML(ShipToList[0].SelectSingleNode("Email").InnerText.ToString()) + "</SHIPTOEMAIL>" + System.Environment.NewLine;

            strXML = strXML + "<SHIPFROMCOMPANY>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantCompany").InnerText.ToString()) + "</SHIPFROMCOMPANY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMCONTACT>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantContact").InnerText.ToString()) + "</SHIPFROMCONTACT>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMADDRESS1>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantAddressLine1").InnerText.ToString()) + "</SHIPFROMADDRESS1>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMADDRESS2>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantAddressLine2").InnerText.ToString()) + "</SHIPFROMADDRESS2>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMADDRESS3></SHIPFROMADDRESS3>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMCITY>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantCity").InnerText.ToString()) + "</SHIPFROMCITY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMSTATE>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantState").InnerText.ToString()) + "</SHIPFROMSTATE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMCOUNTRY>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantCountry").InnerText.ToString()) + "</SHIPFROMCOUNTRY>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMZIP>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantZipCode").InnerText.ToString()) + "</SHIPFROMZIP>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMPHONE>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantPhone").InnerText.ToString()) + "</SHIPFROMPHONE>" + System.Environment.NewLine;
            strXML = strXML + "<SHIPFROMEMAIL>" + removeSplcharactersXML(ShipFromList[0].SelectSingleNode("sPlantEmail").InnerText.ToString()) + "</SHIPFROMEMAIL>" + System.Environment.NewLine;

            strXML = strXML + "<STATUS>Shipped</STATUS>" + System.Environment.NewLine;
            strXML = strXML + "<ERP>SAP Business One</ERP>" + System.Environment.NewLine;
            strXML = strXML + "<PROCESSEDBY>API</PROCESSEDBY>" + System.Environment.NewLine;
            strXML = strXML + "<ESTIMATEDDELIVERYDATE></ESTIMATEDDELIVERYDATE>" + System.Environment.NewLine;


            for (int i = 0; i < Packages.Count; i++)
            {
                XmlNodeList ItemList = Packages[i].SelectNodes("Item");
                XmlNodeList DimensionList = Packages[i].SelectNodes("Dimensions");

                strXML = strXML + "<HU>" + System.Environment.NewLine;
                strXML = strXML + "<HANDLINGUNIT>" + (i + 1).ToString() + "</HANDLINGUNIT>" + System.Environment.NewLine;
                strXML = strXML + "<TRACKINGNUMBER>" + XMLShipRes.GetElementsByTagName("TrackingNumber")[i].InnerText.ToString() + "</TRACKINGNUMBER>" + System.Environment.NewLine;
                strXML = strXML + "<PACKAGENUMBER>" + (i + 1).ToString() + "</PACKAGENUMBER>" + System.Environment.NewLine;
                strXML = strXML + "<WEIGHT>" + Packages[i].SelectSingleNode("WEIGHT").InnerText.ToString() + "</WEIGHT>" + System.Environment.NewLine;
                strXML = strXML + "<WEIGHTUNIT>" + ShipFromList[0].SelectSingleNode("sPlantWeightUnits").InnerText.ToString() + "</WEIGHTUNIT>" + System.Environment.NewLine;
                try
                {
                    string Length = DimensionList[0].SelectSingleNode("Length").InnerText.ToString();
                    string Width = DimensionList[0].SelectSingleNode("Width").InnerText.ToString();
                    string Height = DimensionList[0].SelectSingleNode("Height").InnerText.ToString();
                    string Dimension = Length.Substring(0, Length.IndexOf(".")) + "X" + Width.Substring(0, Width.IndexOf(".")) + "X" + Height.Substring(0, Height.IndexOf("."));

                    strXML = strXML + "<DIMENSIONS>" + Dimension + "</DIMENSIONS>" + System.Environment.NewLine;
                }
                catch (Exception)
                {
                    strXML = strXML + "<DIMENSIONS></DIMENSIONS>" + System.Environment.NewLine;
                }

                strXML = strXML + "<DIMENSIONUNIT>" + ShipFromList[0].SelectSingleNode("sPlantDimUnits").InnerText.ToString() + "</DIMENSIONUNIT>" + System.Environment.NewLine;

                for (int k = 0; k < ItemList.Count; k++)
                {
                    strXML = strXML + "<ITEM>" + System.Environment.NewLine;
                    strXML = strXML + "<ITEM_NO>" + removeSplcharactersXML(ItemList[k].SelectSingleNode("ItemNo").InnerText.ToString()) + "</ITEM_NO>" + System.Environment.NewLine;
                    strXML = strXML + "<ITEM_DESC>" + removeSplcharactersXML(ItemList[k].SelectSingleNode("Description").InnerText.ToString()) + "</ITEM_DESC>" + System.Environment.NewLine;
                    strXML = strXML + "<ITEM_QTY>" + removeSplcharactersXML(ItemList[k].SelectSingleNode("Quantity").InnerText.ToString()) + "</ITEM_QTY>" + System.Environment.NewLine;
                    strXML = strXML + "</ITEM>" + System.Environment.NewLine;
                }
                strXML = strXML + "</HU>" + System.Environment.NewLine;
            }

            strXML = strXML + Forms + System.Environment.NewLine; ;

            strXML = strXML + "</HEADER>" + System.Environment.NewLine;
        }
        catch (Exception Ex)
        {
            strXML = PrepareResponseXML(Ex.Message.ToString());
        }
        
        URL = ConfigurationManager.AppSettings.Get("ManifestService").ToString();

        strResponse = XML_Post(strXML, ConfigurationManager.AppSettings.Get("ManifestService").ToString(), XMLShipReq.GetElementsByTagName("DELIVERY_NUM")[0].InnerText.ToString(), "Manifest");

        return strResponse;
    }
    public string XML_Post(string XMLData,string URL, string Delivery,string ServiceCall)
    {
        
        HttpWebRequest myRequest = default(HttpWebRequest);
        myRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
        myRequest.AllowAutoRedirect = false;
        myRequest.Method = "POST";
        myRequest.ContentType = "text/xml;charset=UTF-8";
        Stream RequestStream = myRequest.GetRequestStream();

        try
        {
            string mydocpath1 = "C:\\ProcessWeaver\\BackUp";
            using (System.IO.StreamWriter outfile =
              new System.IO.StreamWriter(mydocpath1 + @"\"+ ServiceCall + "Request_" + Delivery + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xml"))
            {
                outfile.Write(XMLData);
            }
        }
        catch (Exception)
        {
        }

        byte[] SomeBytes = Encoding.UTF8.GetBytes(XMLData);
        RequestStream.Write(SomeBytes, 0, SomeBytes.Length);
        RequestStream.Close();

        System.Xml.XmlDocument MyXMLrep = default(System.Xml.XmlDocument);
        MyXMLrep = new System.Xml.XmlDocument();

        try
        {
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            if (myResponse.StatusCode == HttpStatusCode.OK)
            {
                //Get the stream. 
                Stream ReceiveStreamFDX = myResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                //send the stream to a reader. 
                StreamReader readStream = new StreamReader(ReceiveStreamFDX, encode);
                //Read the result 
                String Result = readStream.ReadToEnd();
                MyXMLrep.LoadXml(Result);

                try
                {
                    string mydocpath1 = "C:\\ProcessWeaver\\BackUp";
                    using (System.IO.StreamWriter outfile =
                      new System.IO.StreamWriter(mydocpath1 + @"\" + ServiceCall + "Response_" + Delivery + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xml"))
                    {
                        outfile.Write(Result);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception)
        {
        }
        return MyXMLrep.InnerXml.ToString();
    }
    public string removeSplcharactersXML(string strValue)
    {
        strValue = strValue.Replace("&", "&amp;");
        strValue = strValue.Replace("<", "&lt;");
        strValue = strValue.Replace(">", "&gt;");
        strValue = strValue.Replace("\"", "&quot;");
        strValue = strValue.Replace("'", "&apos;");
        strValue = strValue.Replace("(", "");
        strValue = strValue.Replace(")", "");
        return strValue;
    }
}