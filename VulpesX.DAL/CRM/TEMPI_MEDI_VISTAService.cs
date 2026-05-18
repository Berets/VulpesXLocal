using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Models.Production;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.CRM
{
    public interface ITEMPI_MEDI_VISTARepository
    {
        ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID);

        ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID, List<tab_articolo> Articles);
    }

    public class TEMPI_MEDI_VISTARepository : RepositoryBase, ITEMPI_MEDI_VISTARepository
    {
        public TEMPI_MEDI_VISTARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID, List<tab_articolo> Articles)
        {
            throw new NotImplementedException();
        }
    }

    public class TEMPI_MEDI_VISTAUfpRepository : RepositoryBase, ITEMPI_MEDI_VISTARepository
    {
        public TEMPI_MEDI_VISTAUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new ObservableCollection<ProductionModel.ProductionUfpModel>();

                var ciclo_articolo = connection.Query<ANAG_ARTICOLI_CICLO_LAVORAZION>(
                    @"SELECT t.*, 
                            r.repdes as artcicrepDescrizione,
                            f.lavdes as artcicfasDescrizione,
                            f.lavcnc as artcicfascnc 
                            FROM ANAG_ARTICOLI_CICLO_LAVORAZION as t
                        LEFT OUTER JOIN PROD_REPARTI as r ON r.repsoc = t.artcicrepsoc AND r.repcod = t.artcicrep
                        LEFT OUTER JOIN PROD_FASI as f ON f.lavsoc = t.artcicfassoc AND f.lavcod = t.artcicfas
                        WHERE t.ARTCOD=@ArticleID AND t.artcicrepsoc = @CompanyID 
                        ORDER BY artcicseq",
                    new { CompanyID = CompanyID, ArticleID = ArticleID }).ToList();

                if (ciclo_articolo == null || !ciclo_articolo.Any())
                {
                    var analogia_id = connection.Query<string>(@"SELECT artfam FROM ANAG_ARTICOLI WHERE ARTCOD=@ArticleID ", new { ArticleID = ArticleID }).FirstOrDefault();

                    var analogia_ciclo_id = connection.Query<string>(@"SELECT artcicart FROM ANALOGIELEVEL1 WHERE angcod=@AnalogiaID ", new { AnalogiaID = analogia_id }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(analogia_ciclo_id))
                    {
                        var ciclo_analogia = connection.Query<PROD_CICLI_LAVORAZIONE_SEQ>(
                            @"SELECT t.*,
                                    r.repdes as ciccreDescrizione,
                                    f.lavdes as cicfasDescrizione,
                                    f.lavcnc as cicfascnc 
                                FROM PROD_CICLI_LAVORAZIONE_SEQ as t
                                LEFT OUTER JOIN PROD_REPARTI as r ON r.repsoc = t.cicsoc AND r.repcod = t.ciccre
                                LEFT OUTER JOIN PROD_FASI as f ON f.lavsoc = t.cicsoc AND f.lavcod = t.cicfas  
                                WHERE t.cicsoc = @CompanyID AND t.cicart = @AnalogiaCicloID
                                ORDER BY cicseq",
                            new { CompanyID = CompanyID, AnalogiaCicloID = analogia_ciclo_id }).ToList();

                        foreach (var cicart in ciclo_analogia)
                        {
                            var list = connection.Query<(string Commessa, decimal Piazzamento, decimal Produzione)>(
                                  @"SELECT 
                                    t.OrdineCommessa as Commessa, 
                                    SUM(t.TempoPiazzamentoPezzo) as TempoPiazzamentoPezzo, 
                                    SUM(t.TempoProduzionePezzo) as TempoProduzionePezzo 
                                    FROM TempiMedi as t
                                    WHERE t.Societa=@CompanyID AND t.ArticoloID = @ArticleID AND t.RepartoID = @RepartoID AND t.FaseID = @FaseID AND (t.TempoPiazzamentoPezzo > 0 OR t.TempoProduzionePezzo > 0)
                                    GROUP BY t.OrdineCommessa",
                                  new { CompanyID = CompanyID, ArticleID = ArticleID, RepartoID = cicart.ciccre, FaseID = cicart.cicfas });

                            var piazzamento = GetSanitizedTime(list.Where(o => o.Piazzamento > 0).Select(s => s.Piazzamento).ToList());
                            var produzione = GetSanitizedTime(list.Where(o => o.Produzione > 0).Select(s => s.Produzione).ToList());

                            retValue.Add(new ProductionModel.ProductionUfpModel
                            {
                                TipoID = "F",
                                SocietaID = CompanyID,
                                RepartoID = cicart.ciccre,
                                RepartoDescrizione = cicart.ciccreDescrizione,
                                FaseID = cicart.cicfas,
                                FaseDescrizione = cicart.cicfasDescrizione,
                                FaseCNC = cicart.cicfascnc == "S",
                                Sequenza = cicart.cicseq,
                                TempoPiazzamento = piazzamento,
                                TempoProduzione = produzione,
                                TempiMedi = new ObservableCollection<TEMPI_MEDI_VISTA>(list.Select(s => new TEMPI_MEDI_VISTA { Societa = CompanyID, OrdineCommessa = s.Commessa, TempoPiazzamentoPezzo = s.Piazzamento, TempoProduzionePezzo = s.Produzione }))
                            });
                        }
                    }
                }
                else
                {
                    foreach (var cicart in ciclo_articolo)
                    {
                        var list = connection.Query<(string Commessa, decimal Piazzamento, decimal Produzione)>(
                                    @"SELECT 
                                    t.OrdineCommessa as Commessa, 
                                    SUM(t.TempoPiazzamentoPezzo) as TempoPiazzamentoPezzo, 
                                    SUM(t.TempoProduzionePezzo) as TempoProduzionePezzo 
                                    FROM TempiMedi as t
                                    WHERE t.Societa=@CompanyID AND t.ArticoloID = @ArticleID AND t.RepartoID = @RepartoID AND t.FaseID = @FaseID AND (t.TempoPiazzamentoPezzo > 0 OR t.TempoProduzionePezzo > 0)
                                    GROUP BY t.OrdineCommessa",
                                    new { CompanyID = CompanyID, ArticleID = ArticleID, RepartoID = cicart.artcicrep, FaseID = cicart.artcicfas });

                        var piazzamento = GetSanitizedTime(list.Where(o => o.Piazzamento > 0).Select(s => s.Piazzamento).ToList());
                        var produzione = GetSanitizedTime(list.Where(o => o.Produzione > 0).Select(s => s.Produzione).ToList());

                        retValue.Add(new ProductionModel.ProductionUfpModel
                        {
                            TipoID = "A",
                            SocietaID = CompanyID,
                            RepartoID = cicart.artcicrep,
                            RepartoDescrizione = cicart.artcicrepDescrizione,
                            FaseID = cicart.artcicfas,
                            FaseDescrizione = cicart.artcicfasDescrizione,
                            FaseCNC = cicart.artcicfascnc == "S",
                            Sequenza = cicart.artcicseq,
                            TempoPiazzamento = piazzamento,
                            TempoProduzione = produzione,
                            TempiMedi = new ObservableCollection<TEMPI_MEDI_VISTA>(list.Select(s => new TEMPI_MEDI_VISTA { Societa = CompanyID, OrdineCommessa = s.Commessa, TempoPiazzamentoPezzo = s.Piazzamento, TempoProduzionePezzo = s.Produzione }))
                        });
                    }
                }

                return retValue;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ProductionModel.ProductionUfpModel>? GetList(string CompanyID, string ArticleID, List<tab_articolo> Articles)
        {
            try
            {
                using var connection = GetOpenConnection();

                var retValue = new ObservableCollection<ProductionModel.ProductionUfpModel>();

                var analogia_id = connection.Query<string>(@"SELECT artfam FROM ANAG_ARTICOLI WHERE ARTCOD=@ArticleID ", new { ArticleID = ArticleID }).FirstOrDefault();

                var analogia_ciclo_id = connection.Query<string>(@"SELECT artcicart FROM ANALOGIELEVEL1 WHERE angcod=@AnalogiaID ", new { AnalogiaID = analogia_id }).FirstOrDefault();

                if (!string.IsNullOrEmpty(analogia_ciclo_id))
                {
                    var ciclo_analogia = connection.Query<PROD_CICLI_LAVORAZIONE_SEQ>(
                        @"SELECT t.*,
                                    r.repdes as ciccreDescrizione,
                                    f.lavdes as cicfasDescrizione,
                                    f.lavcnc as cicfascnc 
                                FROM PROD_CICLI_LAVORAZIONE_SEQ as t
                                LEFT OUTER JOIN PROD_REPARTI as r ON r.repsoc = t.cicsoc AND r.repcod = t.ciccre
                                LEFT OUTER JOIN PROD_FASI as f ON f.lavsoc = t.cicsoc AND f.lavcod = t.cicfas  
                                WHERE t.cicsoc = @CompanyID AND t.cicart = @AnalogiaCicloID
                                ORDER BY cicseq",
                        new { CompanyID = CompanyID, AnalogiaCicloID = analogia_ciclo_id }).ToList();

                    foreach (var cicart in ciclo_analogia)
                    {
                        List<decimal> piazzamenti = new List<decimal>();
                        List<decimal> produzioni = new List<decimal>();
                        List<TEMPI_MEDI_VISTA> tempi = new List<TEMPI_MEDI_VISTA>();

                        foreach (var article in Articles)
                        {
                            var list = connection.Query<(string Commessa, decimal Piazzamento, decimal Produzione)>(
                                    @"SELECT 
                                        t.OrdineCommessa as Commessa, 
                                        SUM(t.TempoPiazzamentoPezzo) as TempoPiazzamentoPezzo, 
                                        SUM(t.TempoProduzionePezzo) as TempoProduzionePezzo 
                                        FROM TempiMedi as t
                                        WHERE t.Societa=@CompanyID AND t.ArticoloID = @ArticleID AND t.RepartoID = @RepartoID AND t.FaseID = @FaseID AND (t.TempoPiazzamentoPezzo > 0 OR t.TempoProduzionePezzo > 0)
                                        GROUP BY t.OrdineCommessa",
                                    new { CompanyID = CompanyID, ArticleID = article.ID, RepartoID = cicart.ciccre, FaseID = cicart.cicfas });

                            var piazzamento = GetSanitizedTime(list.Where(o => o.Piazzamento > 0).Select(s => s.Piazzamento).ToList());
                            var produzione = GetSanitizedTime(list.Where(o => o.Produzione > 0).Select(s => s.Produzione).ToList());

                            piazzamenti.Add(piazzamento);
                            produzioni.Add(produzione);
                            tempi.AddRange(list.Select(s => new TEMPI_MEDI_VISTA { Societa = CompanyID, OrdineCommessa = s.Commessa, TempoPiazzamentoPezzo = s.Piazzamento, TempoProduzionePezzo = s.Produzione }));
                        }


                        retValue.Add(new ProductionModel.ProductionUfpModel
                        {
                            TipoID = "F",
                            SocietaID = CompanyID,
                            RepartoID = cicart.ciccre,
                            RepartoDescrizione = cicart.ciccreDescrizione,
                            FaseID = cicart.cicfas,
                            FaseDescrizione = cicart.cicfasDescrizione,
                            FaseCNC = cicart.cicfascnc == "S",
                            Sequenza = cicart.cicseq,
                            TempoPiazzamento = piazzamenti.Average(),
                            TempoProduzione = produzioni.Average(),
                            TempiMedi = new ObservableCollection<TEMPI_MEDI_VISTA>(tempi)
                        });
                    }
                }
                return retValue;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        private decimal GetSanitizedTime(List<decimal> Times)
        {
            if (!Times.Any())
                return 0;

            if (Times.Count == 1)
                return Times.Single();

            // CALCOLO MEDIANAE
            if (Times.Count > 1 && Times.Count < 5)
            {
                decimal value = VulpesX.Shared.Utilities.TimeSanitizationHelper.GetMedian(Times.Where(o => o > 0).Select(s => s).ToList());

                return value;
            }

            if (Times.Count >= 5)
            {
                decimal value = VulpesX.Shared.Utilities.TimeSanitizationHelper.FilterByPercentile(Times.Where(o => o > 0).Select(s => s).ToList()).Average(o => o);

                return value;
            }

            return 0;
        }
    }
}
