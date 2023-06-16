using Manager.Model;
using Microsoft.Win32;
using Svg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Manager
{
    /// <summary>
    /// Логика взаимодействия для DataPage.xaml
    /// </summary>
    public partial class DataPage : Page
    {
        bool isShowSuccessfulOperations = true;
        string ImgLoc = "";
        List<Category> categories = new List<Category>();
        List<Level> levels = new List<Level>();
        Dictionary<string, List<Word>> wordTables = new Dictionary<string, List<Word>>();

        public DataPage()
        {
            InitializeComponent();
         

            levels = Data.GetLevels();
            langLevel.ItemsSource = levels;

            UpdateViewsInfoFromDB(); 

            if(categories != null )
                foreach (var category in categories)
                    wordTables.Add(category.CategoriesName, Data.GetWords(category.CategoriesName));
                        
        }

        private void addCategory_Click(object sender, RoutedEventArgs e)
        {
            if(langLevel.SelectedItem != null)
            {
                Level level = (Level)langLevel.SelectedItem;
                if(categoryNameField.Text?.Length > 0)
                {
                    Data.CreateCategory(new Category { Id = 0, LevelsId = level.Id, CategoriesName = categoryNameField.Text }, isShowSuccessfulOperations);

                    UpdateViewsInfoFromDB();
                    wordTables.Add(categoryNameField.Text, new List<Word>());
                }
                else
                {
                    MessageBox.Show("Введите имя категории", "Пустое поле имени категории",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }            
            else
            {
                MessageBox.Show("Виберите уровень языка в списке и повторите попытку", "Не выбран уровень языка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addWord_Click(object sender, RoutedEventArgs e)
        {
            if(categoryNameOfWord.SelectedItem != null)
            {
                if(wordsField.Text?.Length > 0)
                {
                    if(translateWordsField.Text?.Length > 0)
                    {
                        if(transcriptionsField.Text?.Length > 0)
                        {
                            if (sentenceField.Text?.Length > 0)
                            {
                                if (transSentenceField.Text?.Length > 0)
                                {
                                    //if(!(ImgLoc?.Length > 0))
                                    //{
                                    //    ImgLoc = "\\image\\default_picture.png";
                                    //}
                                                                       
                                    
                                    if(imageWord.Source is not null)
                                    {
                                        byte[] image = null;
                                        FileStream file = new FileStream(ImgLoc, FileMode.Open, FileAccess.Read);
                                        BinaryReader binaryReader = new BinaryReader(file);
                                        image = binaryReader.ReadBytes((int)file.Length);

                                        Word word = new Word
                                        {
                                            Id = 0,
                                            CategoryName = categoryNameOfWord.Text,
                                            Words = wordsField.Text.Replace("\'", "\'\'"),
                                            Transcriptions = transcriptionsField.Text.Replace("\'", "\'\'"),
                                            Sentence = sentenceField.Text.Replace("\'", "\'\'"),
                                            TranslateWords = translateWordsField.Text.Replace("\'", "\'\'"),
                                            TransSentence = transSentenceField.Text.Replace("\'", "\'\'"),
                                            Picture = image,
                                            Is_completed = 0
                                        };

                                        Data.AddWord(word, isShowSuccessfulOperations);

                                        if (wordTables[word.CategoryName] != null)
                                        {
                                            wordTables[word.CategoryName] = Data.GetWords(word.CategoryName);

                                            wordCategory.SelectedItem = categories.FirstOrDefault(c => c.CategoriesName == word.CategoryName);

                                            if (wordTables.Keys.Contains(word.CategoryName))
                                                wordGrid.ItemsSource = wordTables[word.CategoryName];
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Добавте картинку", "Пустое поле Picture",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Введите перевод предложения", "Пустое поле \"Перевод предложения\"",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Введите предложение на иностранном", "Пустое поле \"Предложение в оригинале\"",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Введите транскрипцию слова", "Пустое поле \"Транскрипция\"",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Введите перевод слова", "Пустое поле \"Перевод слова\"",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Введите оригинал слова", "Пустое поле \"Слово в оригинале\"",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Виберите категорию слов и повторите попытку", "Не выбрана категория",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void updateCategory_Click(object sender, RoutedEventArgs e)
        {
            if(categoryGrid.SelectedItem != null)
            {
                Category category = (Category)categoryGrid.SelectedItem;
                Level selectedLevelId = (Level)langLevel.SelectedItem;

                Category newCategory = new Category { Id = 0, LevelsId = selectedLevelId.Id, CategoriesName = categoryNameField.Text };

                foreach (var item in categories)
                {
                    if(category.Id == item.Id)
                    {
                        if(newCategory.LevelsId == item.LevelsId && newCategory.CategoriesName == item.CategoriesName)
                        {
                            MessageBox.Show("Не требуется изменение категории, так как данные категории в таблице не изменились");
                        }
                        else
                        {
                            Data.UpdateCategory(newCategory, item, isShowSuccessfulOperations);

                            UpdateViewsInfoFromDB();
                            wordTables.Remove(item.CategoriesName);
                            wordTables.Add(newCategory.CategoriesName, Data.GetWords(newCategory.CategoriesName));

                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберите категорию в таблице и повторите попытку", "Не выбрана категория",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void removeCategory_Click(object sender, RoutedEventArgs e)
        {
            if(categoryGrid.SelectedItem != null)
            {
                var category = (Category)categoryGrid.SelectedItem;

                if (MessageBox.Show("При удалении категории все слова этой категории будут удалены.\n\n\t\tВы уверены?", "Внимание!",
                    MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {                    
                    Data.RemoveCategory(category, isShowSuccessfulOperations);
                }               

                UpdateViewsInfoFromDB();
                wordTables.Remove(category.CategoriesName);
            }
            else
            {
                MessageBox.Show("Виберите категорию в таблице и повторите попытку", "Не выбрана категория для удаления",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void updateWord_Click(object sender, RoutedEventArgs e)
        {
            if(wordGrid.SelectedItem is not null)
            {
                Word updateWord = (Word)wordGrid.SelectedItem;

                updateWord.CategoryName = categoryNameOfWord.SelectedItem.ToString();
                updateWord.Words = wordsField.Text.Replace("\'", "\'\'");
                updateWord.TranslateWords = translateWordsField.Text.Replace("\'", "\'\'");
                updateWord.Transcriptions = transcriptionsField.Text.Replace("\'", "\'\'");
                updateWord.Sentence = sentenceField.Text.Replace("\'", "\'\'");
                updateWord.TransSentence = transSentenceField.Text.Replace("\'", "\'\'");

                byte[] byteArray = null;

                if (imageWord.Source is not null)
                {
                    FileStream file = new FileStream(ImgLoc, FileMode.Open, FileAccess.Read);
                    BinaryReader binaryReader = new BinaryReader(file);
                    byteArray = binaryReader.ReadBytes((int)file.Length);
                }
                updateWord.Picture = byteArray;

                Data.UpdateWord(updateWord, (Word)wordGrid.SelectedItem, isShowSuccessfulOperations);

                if (wordTables.Keys.Contains(updateWord.CategoryName))
                {
                    wordTables[updateWord.CategoryName] = Data.GetWords(updateWord.CategoryName);

                    if (wordCategory.SelectedItem != null)
                    {
                        Category category = (Category)wordCategory.SelectedItem;
                        if (wordTables.Keys.Contains(category.CategoriesName))
                            wordGrid.ItemsSource = wordTables[category.CategoriesName];
                    }
                }
            }
            else
            {
                MessageBox.Show("Виберите слово в таблице и повторите попытку", "Не выбрано слово для редактирования",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void removeWord_Click(object sender, RoutedEventArgs e)
        {
            if (wordGrid.SelectedItem != null)
            {
                Word word = (Word)wordGrid.SelectedItem;
                Data.RemoveWord(word, isShowSuccessfulOperations);

                if (word.CategoryName != null)
                    wordTables[word.CategoryName] = Data.GetWords(word.CategoryName);
            }
            else
            {
                MessageBox.Show("Виберите слово в таблице и повторите попытку", "Не выбрано слово для удаления", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wordCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (wordCategory.SelectedItem != null)
            {
                Category category = (Category)wordCategory.SelectedItem;
                if ( wordTables.Keys.Contains(category.CategoriesName))
                    wordGrid.ItemsSource = wordTables[category.CategoriesName];
            }
        }

        private void isShowSuccOperations_Unchecked(object sender, RoutedEventArgs e)
        {
            isShowSuccessfulOperations = false;
        }

        private void isShowSuccOperations_Checked(object sender, RoutedEventArgs e)
        {
            isShowSuccessfulOperations = true;
        }

        private void wordSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            Category category = (Category)wordCategory.SelectedItem;
            if (category != null)
            {
                List<Word> words = Data.GetWords(category.CategoriesName);

                if (wordSearchField.Text?.Length > 0)
                {
                    wordGrid.ItemsSource = words.FindAll(w => w.Words.ToLower().Contains(wordSearchField.Text.ToLower())
                    || w.TranslateWords.ToLower().Contains(wordSearchField.Text.ToLower()));
                }
                else
                {
                    wordGrid.ItemsSource = words;
                }
            }
        }

        private void categorySearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Category> categoriesTemp = Data.GetCategories();

            if(categorySearchField.Text?.Length > 0)
            {
                categoryGrid.ItemsSource = categoriesTemp.FindAll(c => c.CategoriesName.ToLower().Contains(categorySearchField.Text.ToLower()));
            }
            else
            {
                categoryGrid.ItemsSource = categoriesTemp;
            }
        }

        private void addImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofdPicture = new OpenFileDialog();
                ofdPicture.Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*";
                ofdPicture.FilterIndex = 1;

                if (ofdPicture.ShowDialog() == true)
                {
                    SvgDocument svgDocument = SvgDocument.Open(ofdPicture.FileName);
                    var svgBitmap = svgDocument.Draw();
                    var svgImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        svgBitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    imageWord.Source = svgImage;
                }

                ImgLoc = ofdPicture.FileName.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при загрузке картинки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void wordGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(wordGrid.SelectedItem != null)
            {
                var selectedWord = (Word)wordGrid.SelectedItem;

                categoryNameOfWord.SelectedItem = categories.FirstOrDefault(c => c.CategoriesName == selectedWord.CategoryName);
                wordsField.Text = selectedWord.Words;
                translateWordsField.Text = selectedWord.TranslateWords;
                transcriptionsField.Text = selectedWord.Transcriptions;
                sentenceField.Text = selectedWord.Sentence;
                transSentenceField.Text = selectedWord.TransSentence;
                imageWord.Source = null;                           
            }
        }               

        private void categoryNameOfWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void categoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(categoryGrid.SelectedItem != null)
            {
                var selectedCategory = (Category)categoryGrid.SelectedItem;

                langLevel.SelectedItem = levels.FirstOrDefault(l => l.Id == selectedCategory.LevelsId);
                categoryNameField.Text = selectedCategory.CategoriesName;
            }
        }

        private void UpdateViewsInfoFromDB()
        {
            List<Category> tempCtegory = Data.GetCategories();
            categoryGrid.ItemsSource = tempCtegory.ToList();
            wordCategory.ItemsSource = tempCtegory.ToList();
            categoryNameOfWord.ItemsSource = tempCtegory.ToList();
            categories = tempCtegory.ToList();
        }

        private void langLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
