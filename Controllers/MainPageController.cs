using LiveCharts;
using LiveCharts.Wpf;
using MoveFiles.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using System.Windows.Forms;

namespace MoveFiles.Controllers
{
    public class MainPageController : INotifyPropertyChanged
    {

        #region User_Properties
        private string Regex { get; set; }
        private string Origin { get; set; }
        private string Destination { get; set; }
        private long CheckTime { get; set; }
        #endregion

        #region System_Properties
        private LogFile Log { get; set;}
        System.Timers.Timer timer;
        #endregion

        #region Dashboard_Properties
        private int _quantidadeTransferida { get; set; }
        public int QuantidadeTransferida
        {

            get { return _quantidadeTransferida; }
            set
            {
                if (_quantidadeTransferida != value)
                {
                    _quantidadeTransferida = value;
                    OnPropertyChanged();
                }
            }

        }

        private decimal _quantidadeTransferidaMbytes { get; set; }
        public decimal QuantidadeTransferidaMbytes
        {
            get
            {
                return _quantidadeTransferidaMbytes;
            }
            set
            {
                _quantidadeTransferidaMbytes = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection _pieCollection { get; set; }
        public SeriesCollection PieCollection
        {
            get
            {
                return _pieCollection;
            }
            set
            {
                _pieCollection = value;
                OnPropertyChanged();
            }
        }

        private SeriesCollection _columnCollection { get; set; }
        public SeriesCollection ColumnCollection
        {
            get
            {
                return _columnCollection;
            }
            set
            {
                _columnCollection = value;
                OnPropertyChanged();
            }
        }

        private string[] _columnLabels { get; set; }
        public string[] ColumnLabels
        {
            get { return _columnLabels;}
            set
            {
                _columnLabels = value;
                OnPropertyChanged();
            }
        }
        private Func<double, string> _columnFormatter { get; set; }
        public Func<double, string> ColumnFormatter
        {
            get { return _columnFormatter; }
            set
            {
                _columnFormatter = value;
            }
        }

        Func<ChartPoint, string> labelPoint = chartpoint => string.Format("{0:N2} Mb", chartpoint.Y);

        #endregion


        /// <summary>
        /// Evento criado para facilitar a transição das propriedades para a VIew
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public MainPageController()
        {
            // Configuração do Timer, para repetição contínua do processo
            timer = new System.Timers.Timer();
            timer.Elapsed += Process;
            timer.AutoReset = true;
            timer.Enabled = false;
        }


        /// <summary>
        /// Atualiza o grafico de Pizza. Mostra a quantidade total de arquivos transferidos classificados por tipo (extensao do arquivo).
        /// </summary>
        private void UpdatePieChart()
        {
            var series = new SeriesCollection();
            var statistics = new LogStatistics(Log);
            var dictTotalByExtension = statistics.TotalWeightByExtensions();

            
            foreach (var item in dictTotalByExtension)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {

                    series.Add(new PieSeries()
                    {
                        Title = item.Key,
                        Values = new ChartValues<decimal> { item.Value },
                        DataLabels = true,
                        LabelPoint = labelPoint
                    });
                });
            }
            PieCollection = series;
        }



        /// <summary>
        /// Atualiza o grafico de colunas. Mostra a quantidade total de arquivos transferidos por slot de hora.
        /// </summary>
        private void UpdateColumnChart()
        {
            var series = new SeriesCollection();
            var statistics = new LogStatistics(Log);
            var dictTotalByHour = statistics.TotalWeightByHour();

            var auxValues = new List<decimal>();

            for (int i = 0; i < 24; i++)
            {
                var key = i.ToString();

                if (dictTotalByHour.ContainsKey(key))
                {
                    auxValues.Add(dictTotalByHour[key]);
                }
                else
                {
                    auxValues.Add(0);
                }
            }


            System.Windows.Application.Current.Dispatcher.Invoke(() => {

                series.Add(new ColumnSeries()
                {
                    Title = "Traffic Data",
                    Values = new ChartValues<decimal> (auxValues.ToArray())
                });
            });

            ColumnLabels = new string[] {"00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16","17", "18", "19", "20", "21", "22", "23"};
            ColumnFormatter = value => value.ToString("N");
            ColumnCollection = series;
        }


        /// <summary>
        /// Atualiza os parâmetros de entrada do usuário na classe atual
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="checktime"></param>
        public void UpdateInputsUser(string regex, string origin, string destination, long checktime)
        {
            Regex = regex;
            Origin = origin;
            Destination = destination;
            CheckTime = checktime;
            timer.Interval = CheckTime * 1000;
        }



        /// <summary>
        /// Inicializa o Processo
        /// </summary>
        public void Start()
        {
            timer.Start();
            
        }

        /// <summary>
        /// Para o processo
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }


        /// <summary>
        /// Atualiza todos os gráficos e informações contidas na área de dashboards
        /// </summary>
        private void UpdateDashboard()
        {
            var statistics = new LogStatistics(Log);
            Debug.WriteLine("Quantidade total de arquivos transferidos: " + statistics.TotalFilesTransfer);
            Debug.WriteLine("Quantidade total de arquivos transferidos (MB): " + statistics.TotalFileSizeTransfer);

            UpdatePieChart();
            UpdateColumnChart();
            QuantidadeTransferida = statistics.TotalFilesTransfer;
            QuantidadeTransferidaMbytes = statistics.TotalFileSizeTransfer;
        }


        #region CONTROLE_PROCSSO
        /// <summary>
        /// Responsável por controlar todo o fluxo do processo
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void Process(Object source, System.Timers.ElapsedEventArgs e)
        {
            InitLogFile();
            UpdateDashboard();

            // Se formulário é invalido, informar usuário
            if (!ValidateForm())
            {
                Utils.ShowMessage("Existem entradas inválidas ou nulas no formulário",
                    "Formulário inválido");
                // Para o Processo
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
                // Para o processo
                Stop();
                return;
            }


            // Inicializa um novo pacote de arquivos a serem movidos (LOG)
            var packet = new PacketFilesMoved(Origin, Destination);
           

            foreach (var file_found in list_files_found) {


                // Cria um objeto contendo as principais informaçoes do arquivo (LOG)
                var fileMoved = new FileMoved()
                {
                    FileName = file_found.Substring(Origin.Length+1),
                    FileSize = Utils.GetFileSizeOnDisk(file_found),
                    MovedTime = DateTime.Now
                };

                try
                {
                    //Filtra os filenames dado o input de regex do usuario
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
                    // Para o processo
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

        /// <summary>
        /// Verifica se o nome do @file será filtrado pela entrada do regex
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// True -> O arquivo será filtrado;
        /// False-> O arquivo não será filtrado;
        /// </returns>
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
        /// <summary>
        /// Controla a transferência do arquivo
        /// </summary>
        /// <param name="filename"></param>
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
        /// <summary>
        /// Valida a entrada de dados do usuário
        /// </summary>
        /// <returns>
        /// @False -> Inválido
        /// @True -> Válido
        /// </returns>
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


        /// <summary>
        /// Inicializa o arquivo de Log
        /// </summary>
        private void InitLogFile()
        {
            // Cria o Objeto de Log
            Log = new LogFile();

            // Carrega o arquivo de log e converte para o Objeto
            Load();
        }


        /// <summary>
        /// Converte o arquivo de Log para o obejto de Log
        /// </summary>
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

        /// <summary>
        /// Atualiza o arquivo de LOG
        /// </summary>
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




    }
}
