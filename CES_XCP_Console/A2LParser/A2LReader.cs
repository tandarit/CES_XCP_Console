using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CES_XCP_Console.Model;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Collections;
using System.Collections.ObjectModel;

namespace CES_XCP_Console.A2LParser
{
    public enum A2LModules
    {
        MEASUREMENT,
        CHARACTERISTIC,
        COMPU_METHOD,
        COMPU_VTAB,
        GROUP,
        REF_CHARACTERISTIC
    }

    class A2LReader
    {
        private const string M_ST_START = @"/begin MEASUREMENT";
        private const string M_ST_END = @"/end MEASUREMENT";
        private const string C_ST_START = @"/begin CHARACTERISTIC";
        private const string C_ST_END = @"/end CHARACTERISTIC";
        private const string CM_ST_START = @"/begin COMPU_METHOD";
        private const string CM_ST_END = @"/end COMPU_METHOD";
        private const string CMV_ST_START = @"/begin COMPU_VTAB";
        private const string CMV_ST_END = @"/end COMPU_VTAB";
        private const string GROUP_ST_START = @"/begin GROUP";
        private const string GROUP_ST_END = @"/end GROUP";
        private const string GROUP_RC_START = @"/begin REF_CHARACTERISTIC";
        private const string GROUP_RC_END = @"/end REF_CHARACTERISTIC";

        private const string UBYTE_STRING = "__UBYTE";
        private const string SBYTE_STRING = "__SBYTE";
        private const string UWORD_STRING = "__UWORD";
        private const string SWORD_STRING = "__SWORD";
        private const string ULONG_STRING = "__ULONG";
        private const string SLONG_STRING = "__SLONG";
        private const string A_UINT64_STRING = "__A_UINT64";
        private const string A_INT64_STRING = "__A_INT64";
        private const string FLOAT32_IEEE_STRING = "__FLOAT32_IEEE";
        private const string FLOAT64_IEEE_STRING = "__FLOAT64_IEEE";

        private String a2lfileName;


        private FileStream a2lFileStream;
        private StreamReader a2lStreamReader;
        public string a2lStringLine;

        public A2LReader(string a2lFileName)
        {
            a2lfileName = a2lFileName;            
        }

        

        public ObservableCollection<IBaseModule> GetModuleCollection(A2LModules modules)
        {
            int firstLocation, lengthSubString;
            string line;
            string[] lineElements;
            CHARACTERISTIC tempCharacteristic;
            GROUP tempGroup;
            COMPU_VTAB tempCompVtab;
            List<string> referanceString;
            Dictionary<ulong, string> localCompvtabDict;
            IBaseModule module;
            ObservableCollection<IBaseModule> localModuleList = new ObservableCollection<IBaseModule>();

            if (File.Exists(a2lfileName))
            {                
                a2lFileStream = new FileStream(a2lfileName, FileMode.Open);
                a2lStreamReader = new StreamReader(a2lFileStream);

                switch (modules)
                {
                    case A2LModules.CHARACTERISTIC:
                        while ((line = a2lStreamReader.ReadLine()) != null)
                        {
                            if (line.Contains(C_ST_START))
                            {
                                tempCharacteristic = new CHARACTERISTIC();
                                lineElements = line.Split(' ');
                                tempCharacteristic.Name = lineElements[2];
                                firstLocation = line.IndexOf("\"") + 1;
                                lengthSubString = (line.Length - 1) - firstLocation;
                                tempCharacteristic.Description = line.Substring(firstLocation, lengthSubString);

                                line = a2lStreamReader.ReadLine();

                                //többsoros comment esetén * -al kezdődik minden
                                if (line.Contains('*') == true)
                                {
                                    tempCharacteristic.Description += line.Substring(1, line.Length - 2);
                                    line = a2lStreamReader.ReadLine();

                                }

                                lineElements = line.Split(' ');
                                tempCharacteristic.Address = Convert.ToUInt32(lineElements[1], 16);
                                tempCharacteristic.VariableType = lineElements[2].Substring(2);
                                tempCharacteristic.ConversionType = lineElements[4];
                                tempCharacteristic.isReadOnly = false;
                                if (line.Contains("VAL_BLK"))
                                {
                                    line = a2lStreamReader.ReadLine();

                                    lineElements = line.Split(' ');
                                    tempCharacteristic.ArraySize = Convert.ToDecimal(lineElements[1]);
                                }
                                else
                                {
                                    tempCharacteristic.ArraySize = 0;
                                }

                                line = a2lStreamReader.ReadLine();

                                if (!line.Contains(C_ST_END) && line.Contains("READ_ONLY"))
                                    tempCharacteristic.isReadOnly = true;

                                module = tempCharacteristic;
                                localModuleList.Add(module);
                            }
                        }
                        break;

                    case A2LModules.GROUP:
                        while ((line = a2lStreamReader.ReadLine()) != null)
                        {
                            if (line.Contains(GROUP_ST_START))
                            {
                                tempGroup = new GROUP();
                                referanceString = new List<string>();
                                lineElements = line.Split(' ');
                                tempGroup.Name = lineElements[2];
                                firstLocation = line.IndexOf("\"") + 1;
                                lengthSubString = (line.Length - 1) - firstLocation;
                                tempGroup.Description = line.Substring(firstLocation, lengthSubString);

                                line = a2lStreamReader.ReadLine();

                                //többsoros comment esetén * -al kezdődik minden
                                if (line.Contains('*') == true)
                                {
                                    tempGroup.Description += line.Substring(1, line.Length - 2);
                                    line = a2lStreamReader.ReadLine();

                                }
                                line = a2lStreamReader.ReadLine();


                                while (!(line = a2lStreamReader.ReadLine()).Contains(GROUP_RC_END))
                                {
                                    referanceString.Add(line.Substring(4));
                                }

                                tempGroup.RefCharacteristicList = referanceString;
                                module = tempGroup;
                                localModuleList.Add(module);
                            }
                        }
                        break;

                    case A2LModules.COMPU_VTAB:
                        while ((line = a2lStreamReader.ReadLine()) != null)
                        {
                            if (line.Contains(CMV_ST_START))
                            {
                                localCompvtabDict = new Dictionary<ulong, string>();
                                tempCompVtab = new COMPU_VTAB();
                                lineElements = line.Split(' ');
                                tempCompVtab.Name = lineElements[2];
                                firstLocation = line.IndexOf("\"") + 1;
                                lengthSubString = (line.Length - 1) - firstLocation;
                                tempCompVtab.Description = line.Substring(firstLocation, lengthSubString);

                                line = a2lStreamReader.ReadLine();

                                //többsoros comment esetén * -al kezdődik minden
                                if (line.Contains('*') == true)
                                {
                                    tempCompVtab.Description += line.Substring(1, line.Length - 2);
                                    line = a2lStreamReader.ReadLine();

                                }

                                lineElements = line.Split(' ');
                                tempCompVtab.Count = Convert.ToInt32(lineElements[1]);

                                for (int i = 0; i < tempCompVtab.Count; i++)
                                {
                                    line = a2lStreamReader.ReadLine();
                                    lineElements = line.Split(' ');
                                    localCompvtabDict.Add(Convert.ToUInt64(lineElements[0]), lineElements[1]);
                                }
                                tempCompVtab.Instance = localCompvtabDict;
                                module = tempCompVtab;
                                localModuleList.Add(module);
                            }
                        }
                        break;
                }

                a2lStreamReader.Close();
                a2lFileStream.Close();
            }
            else
            {
                a2lStreamReader = null;
                a2lFileStream = null;
                return null;
            }

            return localModuleList;
        }
               

        public ObservableCollection<GROUP> GetGroupList()
        {
            int firstLocation, lengthSubString;
            string line;
            string[] lineElements;
            GROUP tempGroup;
            ObservableCollection<GROUP> localRefCharacteristicList = new ObservableCollection<GROUP>();
            List<string> referanceString;
           

            if (File.Exists(a2lfileName))
            {
                //A2LLineNumber = File.ReadLines(a2lfileName).Count();
                a2lFileStream = new FileStream(a2lfileName, FileMode.Open);
                a2lStreamReader = new StreamReader(a2lFileStream);           

                while ((line = a2lStreamReader.ReadLine()) != null)
                {
                    
                    if (line.Contains(GROUP_ST_START))
                    {
                        tempGroup = new GROUP();
                        referanceString = new List<string>();
                        lineElements = line.Split(' ');
                        tempGroup.Name = lineElements[2];
                        firstLocation = line.IndexOf("\"") + 1;
                        lengthSubString = (line.Length - 1) - firstLocation;
                        tempGroup.Description = line.Substring(firstLocation, lengthSubString);

                        line = a2lStreamReader.ReadLine();
                       
                        //többsoros comment esetén * -al kezdődik minden
                        if (line.Contains('*') == true)
                        {
                            tempGroup.Description += line.Substring(1, line.Length - 2);
                            line = a2lStreamReader.ReadLine();
                           
                        }
                        line = a2lStreamReader.ReadLine();
                        
                        
                        while (!(line = a2lStreamReader.ReadLine()).Contains(GROUP_RC_END))
                        {
                            referanceString.Add(line.Substring(4));
                        }

                        tempGroup.RefCharacteristicList = referanceString;
                        localRefCharacteristicList.Add(tempGroup);
                    }
                }
                a2lStreamReader.Close();
                a2lFileStream.Close();
                return localRefCharacteristicList;
            }
            else
            {
                a2lStreamReader=null;
                a2lFileStream=null;
                return null;
            }
        }


        public ObservableCollection<COMPU_VTAB> GetCompuVtabList()
        {
            int firstLocation, lengthSubString;
            string line;
            string[] lineElements;
            COMPU_VTAB tempCompVtab;
            ObservableCollection<COMPU_VTAB> localCompvtabList = new ObservableCollection<COMPU_VTAB>();
            Dictionary<ulong, string> localCompvtabDict;
            

            if (File.Exists(a2lfileName))
            {
                //A2LLineNumber = File.ReadLines(a2lfileName).Count();
                a2lFileStream = new FileStream(a2lfileName, FileMode.Open);
                a2lStreamReader = new StreamReader(a2lFileStream);

                while ((line = a2lStreamReader.ReadLine()) != null)
                {
                    
                    if (line.Contains(CMV_ST_START))
                    {
                        localCompvtabDict = new Dictionary<ulong, string>();
                        tempCompVtab = new COMPU_VTAB();
                        lineElements = line.Split(' ');
                        tempCompVtab.Name = lineElements[2];
                        firstLocation = line.IndexOf("\"") + 1;
                        lengthSubString = (line.Length - 1) - firstLocation;
                        tempCompVtab.Description = line.Substring(firstLocation, lengthSubString);

                        line = a2lStreamReader.ReadLine();
                        
                        //többsoros comment esetén * -al kezdődik minden
                        if (line.Contains('*') == true)
                        {
                            tempCompVtab.Description += line.Substring(1, line.Length - 2);
                            line = a2lStreamReader.ReadLine();
                           
                        }

                        lineElements = line.Split(' ');
                        tempCompVtab.Count = Convert.ToInt32(lineElements[1]);

                        for (int i = 0; i < tempCompVtab.Count; i++)
                        {
                            line = a2lStreamReader.ReadLine();                            
                            lineElements = line.Split(' ');
                            localCompvtabDict.Add(Convert.ToUInt64(lineElements[0]), lineElements[1]);
                        }
                        tempCompVtab.Instance = localCompvtabDict;
                        localCompvtabList.Add(tempCompVtab);
                    }
                }
                a2lStreamReader.Close();
                a2lFileStream.Close();
                return localCompvtabList;
            }
            else
            {
                a2lStreamReader = null;
                a2lFileStream = null;
                return null;
            }
        }

        public ObservableCollection<CHARACTERISTIC> GetCharacteristicList()
        {
            int firstLocation, lengthSubString;
            string line;
            string[] lineElements;
            CHARACTERISTIC tempCharacteristic;
            ObservableCollection<CHARACTERISTIC> localCharateristicList = new ObservableCollection<CHARACTERISTIC>();
            

            if (File.Exists(a2lfileName))
            {
                //A2LLineNumber = File.ReadLines(a2lfileName).Count();
                a2lFileStream = new FileStream(a2lfileName, FileMode.Open);
                a2lStreamReader = new StreamReader(a2lFileStream);

                while ((line = a2lStreamReader.ReadLine()) != null)
                {
                    
                    if (line.Contains(C_ST_START))
                    {
                        tempCharacteristic = new CHARACTERISTIC();
                        lineElements = line.Split(' ');
                        tempCharacteristic.Name = lineElements[2];
                        firstLocation = line.IndexOf("\"") + 1;
                        lengthSubString = (line.Length - 1) - firstLocation;
                        tempCharacteristic.Description = line.Substring(firstLocation, lengthSubString);

                        line = a2lStreamReader.ReadLine();
                        
                        //többsoros comment esetén * -al kezdődik minden
                        if (line.Contains('*') == true)
                        {
                            tempCharacteristic.Description += line.Substring(1, line.Length - 2);
                            line = a2lStreamReader.ReadLine();
                            
                        }

                        lineElements = line.Split(' ');
                        tempCharacteristic.Address = Convert.ToUInt32(lineElements[1], 16);
                        tempCharacteristic.VariableType = lineElements[2].Substring(2);
                        tempCharacteristic.ConversionType = lineElements[4];
                        tempCharacteristic.isReadOnly = false;
                        if (line.Contains("VAL_BLK"))
                        {
                            line = a2lStreamReader.ReadLine();
                            
                            lineElements = line.Split(' ');
                            tempCharacteristic.ArraySize = Convert.ToDecimal(lineElements[1]);
                        }
                        else
                        {
                            tempCharacteristic.ArraySize = 0;
                        }

                        line = a2lStreamReader.ReadLine();
                       
                        if (!line.Contains(C_ST_END) && line.Contains("READ_ONLY"))
                            tempCharacteristic.isReadOnly = true;

                        localCharateristicList.Add(tempCharacteristic);
                    }
                }
                a2lStreamReader.Close();
                a2lFileStream.Close();
                return localCharateristicList;
            }
            else
            {
                a2lStreamReader = null;
                a2lFileStream = null;
                return null;
            }
        }
    }
}
