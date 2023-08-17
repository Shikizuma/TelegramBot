using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;
using Excel = Microsoft.Office.Interop.Excel.Application;

namespace TelegramBot
{
	internal class ExcelApp
	{
        Excel application;
        Workbook book;
        Worksheet sheet;

		public ExcelApp(string path)
        {
            application = new Excel();
            book = application.Workbooks.Open(path);
            application.Visible = true;
        }

        public QuestionModel[] GetQuestions()
        {
            if (book == null)
                return new QuestionModel[0];

			sheet = book.Sheets["Questions"];
            int count = Count;
			QuestionModel[] questions = new QuestionModel[Count];
            for (int i = 0; i < questions.Length; i++)
            {
                questions[i] = new QuestionModel()
                {
                    Question = sheet.Cells[i + 1, "A"].Text,
                    Responce = sheet.Cells[i + 1, "B"].Text,
                };
            }
            return questions;
		}

		public FilmModel[] GetFilms()
		{
			if (book == null)
				return new FilmModel[0];

			sheet = book.Sheets["Films"];
			int count = Count;
			FilmModel[] films = new FilmModel[Count];
			for (int i = 0; i < films.Length; i++)
			{
				films[i] = new FilmModel()
				{
					Name = sheet.Cells[i + 1, "A"].Text,
					Description = sheet.Cells[i + 1, "B"].Text,
					Genre = sheet.Cells[i + 1, "C"].Text,
					Rate = sheet.Cells[i + 1, "D"].Value2,
					Views = sheet.Cells[i + 1, "E"].Value2,
					Image = sheet.Cells[i + 1, "F"].Text,
				};
			}
			return films;
		}

		public int Count
        {
            get
            {
                int count = 0;
                if (sheet == null)
                    return -1;

                while (true)
                {
                    var value = sheet.Cells[count + 1, "A"].Value2;
                    if (value == null)
                        break;
                    count++;
                }
                return count;
            }
        }
    }
}
