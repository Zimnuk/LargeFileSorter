using iluvadev.ConsoleProgressBar;

namespace LargeFileSorter;

public class DetailedProgressBar : IDisposable
{
    public ProgressBar ProgressBar { get; set; }

    public DetailedProgressBar(int max, string? elementText, string? doneText)
    {
        ProgressBar = new ProgressBar(){ Maximum = max};
        ProgressBar.Text.Description.Clear();
        ProgressBar.Text.Description.Processing.AddNew().SetValue(pb => $"{elementText??"Element"}: {pb.ElementName}");
        ProgressBar.Text.Description.Done.AddNew().SetValue(pb => $"{pb.Value} {doneText?? "elements"} in {pb.TimeProcessing.TotalSeconds}s.");
    }

    public void Update(string elementName)
    {
        ProgressBar.PerformStep(elementName);
    }

    public void Dispose()
    {
        ProgressBar.Dispose();
    }
}