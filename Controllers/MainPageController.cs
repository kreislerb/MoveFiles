using MoveFiles.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public MainPageController(string regex, string origin, string destination, long checktime)
        {
            Regex = regex;
            Origin = origin;
            Destination = destination;
            CheckTime = checktime;
            InitLogFile();
        }


        public void StartProcess()
        {

            // Se formulário é invalido, informar usuário
            if (!ValidateForm())
            {
                Utils.ShowMessage("Existem entradas inválidas ou nulas no formulário",
                    "Formulário inválido");
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
                    //Move arquivo para pasta de destino
                    SendFileToDestination(fileMoved.FileName);

                    // Insere o arquivo no pacote de log
                    packet.InsertFileMove(fileMoved);

                }
                catch (Exception err)
                {
                    Utils.ShowMessage(err.Message, "Falha ao mover arquivo");
                    return;
                }
            }


            Log.InsertPacketFilesMoved(packet);

            if(Log.CountPackets > 0)
            {
                Upsert();
            }
            



        }

      


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
                Upsert();
            }

        }


        public void Upsert()
        {

            //open file stream
            using (StreamWriter file = File.CreateText(@"C:\LogFile.json"))
            {
                JsonSerializer serializer = new JsonSerializer();

                //serialize object directly into file stream
                serializer.Serialize(file, Log);
            }

        }


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



        private bool ValidateForm()
        {
            // Validar campos nulos
            if (string.IsNullOrEmpty(Regex) || string.IsNullOrEmpty(Origin) || string.IsNullOrEmpty(Destination))
                return false;

            // Validar CheckTime

            if (CheckTime <= 0)
                return false;


            return true;
        }






        public void SaveConfigs()
        {


        }


        public void LoadConfigs() { 
        
        
        }




    }
}
