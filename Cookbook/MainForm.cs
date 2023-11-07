using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Cookbook
{
    public partial class MainForm : Form
    {
        string strConnection;
        SqlConnection connDb;

        public MainForm()
        {
            InitializeComponent();
            strConnection = ConfigurationManager.ConnectionStrings["Cookbook.Properties.Settings.CookbookConnectionString"].ConnectionString;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PopulateRecipes();
            PopulateAllIngredients();
        }

        private void PopulateRecipes()
        {
            string strRecipeQuery = "SELECT * FROM Recipe";
            using (connDb = new SqlConnection(strConnection))
            using (SqlDataAdapter adapterData = new SqlDataAdapter(strRecipeQuery, connDb))
            {
                DataTable table = new DataTable();
                adapterData.Fill(table);

                listRecipes.DisplayMember = "Name";
                listRecipes.ValueMember = "Id";
                listRecipes.DataSource= table;
            }
        }

        private void PopulateAllIngredients()
        {
            string strRecipeQuery = "SELECT * FROM Ingredient";
            using (connDb = new SqlConnection(strConnection))
            using (SqlDataAdapter adapterData = new SqlDataAdapter(strRecipeQuery, connDb))
            {
                DataTable table = new DataTable();
                adapterData.Fill(table);

                listAllIngredients.DisplayMember = "Name";
                listAllIngredients.ValueMember = "Id";
                listAllIngredients.DataSource = table;
            }
        }

        private void PopulateIngredients()
        {
            string strQuery = "SELECT a.Name FROM Ingredient a " +
                "INNER JOIN RecipeIngredient b ON a.Id = b.IngredientId " +
                "WHERE b.RecipeId = @RecipeId";

            using (connDb = new SqlConnection(strConnection))
            using (SqlCommand command = new SqlCommand(strQuery, connDb))
            using (SqlDataAdapter adapterData = new SqlDataAdapter(command))
            {
                command.Parameters.AddWithValue("@RecipeId", listRecipes.SelectedValue);

                DataTable tableIngredients = new DataTable();
                adapterData.Fill(tableIngredients);

                listIngredients.DisplayMember = "Name";
                listIngredients.ValueMember = "Id";
                listIngredients.DataSource = tableIngredients;
            }
        }

        private void listRecipes_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateIngredients();
        }

        private void btnAddRecipe_Click(object sender, EventArgs e)
        {
            string strQuery = "INSERT INTO Recipe VALUES (@RecipeName, 80, 'instructions')";

            using (connDb = new SqlConnection(strConnection))
            using (SqlCommand command = new SqlCommand(strQuery, connDb))
            {
                connDb.Open();
                command.Parameters.AddWithValue("@RecipeName", textRecipeName.Text);
                command.ExecuteNonQuery();
            }

            PopulateRecipes();
        }

        private void btnUpdateRecipeName_Click(object sender, EventArgs e)
        {
            string strQuery = "UPDATE Recipe SET Name = @RecipeName WHERE Id = @RecipeId";

            using (connDb = new SqlConnection(strConnection))
            using (SqlCommand command = new SqlCommand(strQuery, connDb))
            {
                connDb.Open();
                command.Parameters.AddWithValue("@RecipeName", textRecipeName.Text);
                command.Parameters.AddWithValue("@RecipeId", listRecipes.SelectedValue);
                command.ExecuteNonQuery();
            }

            PopulateRecipes();
        }

        private void btnAddToRecipe_Click(object sender, EventArgs e)
        {
            string strQuery = "INSERT INTO RecipeIngredient VALUES (@RecipeId, @IngredientId)";

            using (connDb = new SqlConnection(strConnection))
            using (SqlCommand command = new SqlCommand(strQuery, connDb))
            {
                connDb.Open();
                command.Parameters.AddWithValue("@RecipeId", listRecipes.SelectedValue);
                command.Parameters.AddWithValue("@IngredientId", listAllIngredients.SelectedValue);
                command.ExecuteNonQuery();
            }

            PopulateIngredients();
        }
    }
}
