using DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace testDiplom
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> comparisonSigns = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            comparisonSigns["меньше"] = "<";
            comparisonSigns["больше"] = ">";
            comparisonSigns["не меньше"] = ">=";
            comparisonSigns["не больше"] = "<=";

            comparisonSigns["раньше"] = "<";
            comparisonSigns["позже"] = ">";
            comparisonSigns["не раньше"] = ">=";
            comparisonSigns["не позже"] = "<=";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            diplomContext context = new diplomContext();

            //город (Страна, =, Канада) (Колич-жителей, Не больше, 50000)
            String[] parts = input.Text.Split('*');

            String entity = parts[0];

            var translateEntity = translate(context, entity);

            String typeEntity = createHeader(translateEntity);

            String querySparql = typeEntity;

            int numVar = 3;

            parts = parts.Skip(1).ToArray();

            foreach (String part in parts)
            {
                String[] triple = part.Replace("(", "").Replace(")", "").Split(",");
                var translatePredicate = translate(context, triple[0]);

                var translateValue = translate(context, triple[2]);

                if (triple[1].Equals("="))
                {
                    querySparql += createEqTriple(translatePredicate, numVar++, translateValue.First());
                }
                else
                {
                    querySparql += createCompareTriple(translatePredicate, numVar++, comparisonSigns[triple[1].ToLower()], triple[2]);
                }

            }
            querySparql += "}";

            output.Text = querySparql;

            var res = runQuery(querySparql);

            foreach (String str in res) {
                results.Items.Add(new {  Value = str });
            }

            
        }

        private String createValues(List<String> values)
        {
            return "{" + String.Join(" ", values) + "}";
        }

        private String createHeader(List<String> values)
        {
            return "select distinct ?var1 where { values ?var2 " + createValues(values) + " . ?var1 rdf:type ?var2.";
        }

        //todo добавить разницу значений числовых, даты и типа dbr:Canada
        private String createEqTriple(List<String> values, int numPredicate, String valuePredicate)
        {
            return " values ?p" + numPredicate.ToString() + " " + createValues(values) + " . ?var1 ?p" + numPredicate.ToString() + " " + valuePredicate + ".";
        }

        private String createCompareTriple(List<String> values, int numPredicate, String comparisonSign, String comparisonValue)//TODO  
        {
            return " values ?p" + numPredicate.ToString() + " " + createValues(values) + " . ?var1 ?p" + numPredicate.ToString() + " ?var" + numPredicate.ToString() + "."
                + "filter (?var" + numPredicate.ToString() + " " + comparisonSign + " " + comparisonValue + ")";
        }

        private List<String> translate(diplomContext context, String word)
        {
            return (from translation in context.NameOntologyPredicates
                    where translation.IdNameReprNavigation.NameRepr.ToLower().Equals(word.ToLower())
                    select translation.NamePredicate).ToList();
        }

        private List<String> runQuery(String commandText)
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();

            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("dbr", new Uri("http://dbpedia.org/resource/"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("schema", new Uri("http://schema.org/"));

            queryString.CommandText = commandText;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);

            //IGraph g = new Graph();
            //UriLoader.Load(g, new Uri("http://dbpedia.org/resource/"));

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            SparqlResultSet queryResults = endpoint.QueryWithResultSet(queryString.ToString());

            List<String> results = new List<String>();

            foreach (SparqlResult row in queryResults)
            {
                results.Add(row.Value("var1").ToString());
            }

            return results;
        }
    }
}
