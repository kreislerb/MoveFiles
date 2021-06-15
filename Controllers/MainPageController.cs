using MoveFiles.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

using System.Windows.Forms;

namespace MoveFiles.Controllers
{
    public class MainPageController
    {

        private string Regex { get; set; }
        private string Origin { get; set; }
        private string Destination { get; set; }
        private long CheckTime { get; set; }

        private LogFile Log { get; set;}
        System.Timers.Timer timer;
        
        public MainPageController()
        {

          
            timer = new System.Timers.Timer();
            timer.Elapsed += Process;
            timer.AutoReset = true;
            timer.Enabled = false;

            
        }


        public void UpdateInputsUser(string regex, string origin, string destination, long checktime)
        {
            Regex = regex;
            Origin = origin;
            Destination = destination;
            CheckTime = checktime;
            timer.Interval = CheckTime * 1000;
        }



        public void Start()
        {
            timer.Start();
            
        }
        public void Stop()
        {
            timer.Stop();
        }



        


        #region CONTROLE_PROCSSO
        private void Process(Object source, System.Timers.ElapsedEventArgs e)
        {
            InitLogFile();
            var statistics = new LogStatistics(Log);
            Debug.WriteLine("Quantidade total de arquivos transferidos: " + statistics.TotalFilesTransfer);
            Debug.WriteLine("Quantidade total de arquivos transferidos (MB): " + statistics.TotalFileSizeTransfer);
            statistics.TotalWeightByExtensions();
            statistics.TotalWeightByHour();

            // Se formulário é invalido, informar usuário
            if (!ValidateForm())
            {
                Utils.ShowMessage("Existem entradas inválidas ou nulas no formulário",
                    "Formulário inválido");
                Stop();
                return;
            }

            string[] list_files_found;

            // Validar diretorio de origem
            try
            {
                list_files_found = Directory.GetFiles(Origin);
            }
            catch (Exception err) {

                Utils.ShowMessage("Diretório de origem não é válido!",
                    "Formulário inválido");
                Stop();
                return;
            }


            var packet = new PacketFilesMoved(Origin, Destination);
           
            foreach (var file_found in list_files_found) {

                var fileMoved = new FileMoved()
                {
                    FileName = file_found.Substring(Origin.Length+1),
                    FileSize = Utils.GetFileSizeOnDisk(file_found),
                    MovedTime = DateTime.Now
                };

                try
                {
                    //FilterByRegex
                    if (!FilterByRegex(fileMoved))
                    {
                        //Move arquivo para pasta de destino
                        SendFileToDestination(fileMoved.FileName);

                        // Insere o arquivo no pacote de log
                        packet.InsertFileMove(fileMoved);
                        
                    }

                }
                catch (Exception err)
                {
                    Utils.ShowMessage(err.Message, "Falha ao mover arquivo");
                    Stop();
                    return;
                }

            }


            
            // Insere na o pacote de dados de log na lista e salva
            Log.InsertPacketFilesMoved(packet);

            if(list_files_found.Length> 0)
            {
                Upsert();
            }
            
        }


        #endregion


        private bool FilterByRegex(FileMoved file)
        {
            if (string.IsNullOrEmpty(Regex))
                return false;
            
            try
            {

            
            Regex reg = new Regex(Regex);
            if (reg.IsMatch(file.FileName))
            {
                return false;
            }
            return true;
            }

            catch (ArgumentException err)
            {
                throw new ArgumentException("Regex Inválido!");
            }
        }


        #region MOVER_ARQUIVOS

        private void SendFileToDestination(string filename)
        {
            try
            {
                var originFilePath = Origin + "\\" + filename;
                var destinationFilePath = Destination + "\\" + filename;
                File.Move(originFilePath, destinationFilePath);

            }
            catch (FileNotFoundException err)
            {
                throw new FileNotFoundException("O arquivo \"" + filename + " não foi encontrado!");
            }
            catch (ArgumentNullException err)
            {
                throw new ArgumentNullException("O diretório de origem ou destino não foram informados!");
            }
            catch (ArgumentException err)
            {
                throw new ArgumentException("O diretório de origem ou destino não foram informados!");
            }
            catch (UnauthorizedAccessException err)
            {
                throw new UnauthorizedAccessException("É necessário ter permissão do Administrador para realizar esta operação!");
            }
            catch (PathTooLongException err)
            {
                throw new PathTooLongException(err.Message);
            }
            catch (DirectoryNotFoundException err)
            {
                throw new DirectoryNotFoundException("Diretório informado é inválido!");
            }
            catch (NotSupportedException err)
            {
                throw new NotSupportedException("O nome do diretório ou arquivo \"" + filename + "\" não é válido!");
            }

            catch (IOException err)
            {
                throw new IOException("O arquivo \"" + filename + " Já existe no diretório de destino");
            }

        }
        #endregion

        #region VALIDAÇÃO
        private bool ValidateForm()
        {
            // Validar campos nulos
            if (string.IsNullOrEmpty(Origin) || string.IsNullOrEmpty(Destination))
                return false;

            // Validar CheckTime

            if (CheckTime <= 0)
                return false;


            return true;
        }

        #endregion

        #region LOG_FILE


        private void InitLogFile()
        {
            // Cria o Objeto de Log
            Log = new LogFile();

            // Carrega o arquivo de log e converte para o Objeto
            Load();
        }

        public void Load()
        {
            try
            {
                using (StreamReader r = new StreamReader(@"C:\LogFile.json"))
                {
                    string json = r.ReadToEnd();
                    Log = JsonConvert.DeserializeObject<LogFile>(json);
                }
            }
            catch (Exception err)
            {
                
            }

        }


        public void Upsert()
        {
            try
            {
                using (StreamWriter file = File.CreateText(@"C:\LogFile.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    //serialize object directly into file stream
                    serializer.Serialize(file, Log);
                }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                bool arquivoEmUso = errorCode == 32 || errorCode == 33;
            }

            
           

        }


        #endregion





        public void SaveConfigs()
        {


        }


        public void LoadConfigs() { 
        
        
        }




    }
}
