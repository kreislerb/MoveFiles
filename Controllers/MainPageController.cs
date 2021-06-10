using MoveFiles.Models;
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

        public MainPageController(string regex, string origin, string destination, long checktime)
        {
            Regex = regex;
            Origin = origin;
            Destination = destination;
            CheckTime = checktime;
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

            foreach (var file_found in list_files_found) {

                var fileMoved = new FileMoved()
                {
                    FileName = file_found,
                    FileSize = Utils.GetFileSizeOnDisk(file_found),
                    MovedTime = DateTime.Now
                };


            }


        }



        public void SendFileToDestination(string filepath)
        {
            try
            {

               

            }
           

            catch (FileNotFoundException err)
            {
                Utils.ShowMessage("O arquivo \"" + filepath + " não foi encontrado!",
                 "Falha ao enviar");
            }
            catch (ArgumentNullException err)
            {
                Utils.ShowMessage("O diretório de origem ou destino não foram informados!",
                "Falha ao enviar");
            }
            catch (ArgumentException err)
            {
                Utils.ShowMessage("O diretório de origem ou destino não foram informados!",
               "Falha ao enviar");
            }
            catch (UnauthorizedAccessException err)
            {
                Utils.ShowMessage("É necessário ter permissão do Administrador para realizar esta operação!",
             "Falha ao enviar");
            }
            catch (PathTooLongException err)
            {
                Utils.ShowMessage(err.Message,
            "Falha ao enviar");
            }
            catch (DirectoryNotFoundException err)
            {
                Utils.ShowMessage("Diretório informado é inválido!",
            "Falha ao enviar");
            }
            catch (NotSupportedException err)
            {
                Utils.ShowMessage("O nome do diretório ou arquivo \""+ filepath +"\" não é válido!" ,
          "Falha ao enviar");
            }

            catch (IOException err)
            {
                Utils.ShowMessage("O arquivo \"" + filepath + " Já existe no diretório de destino",
                   "Falha ao enviar");
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
