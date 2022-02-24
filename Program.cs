var dataset = Dataset.Parse("input/a_an_example.in.txt");

System.Console.WriteLine("Done");

public class Contributor
{
    public string Name { get; set; }
    public Dictionary<string, int> Skills { get; set; } = new();
}

public class Project
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public int Score { get; set; }
    public int Deadline { get; set; }
    public Dictionary<string, int> Roles { get; set; } = new();

}

public class Dataset
{
    public Dataset(List<Contributor> contributors, List<Project> projects)
    {
        Contributors = contributors;
        Projects = projects;
    }

    public List<Contributor> Contributors { get; set; } = new();
    public List<Project> Projects { get; set; } = new();

    public static Dataset Parse(string path)
    {
        var lines = File.ReadAllLines(path);

        var firstLine = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var contributorsCount = int.Parse(firstLine[0]);
        var projectsCount = int.Parse(firstLine[1]);

        var contributors = new List<Contributor>();

        int i = 1;
        for (int c = 0; c < contributorsCount; c++)
        {
            var contributor = new Contributor();

            var contributorLine = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            contributor.Name = contributorLine[0];
            var skillsCount = int.Parse(contributorLine[1]);

            for (int j = 0; j < skillsCount; j++)
            {
                var skillLine = lines[i + j + 1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var name = skillLine[0];
                var level = int.Parse(skillLine[1]);

                contributor.Skills.Add(name, level);
            }

            i += skillsCount + 1;

            contributors.Add(contributor);
        }

        var projects = new List<Project>();

        for (int r = 0; r < projectsCount; r++)
        {
            var project = new Project();

            var projectLine = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            project.Name = projectLine[0];
            project.Duration = int.Parse(projectLine[1]);
            project.Score = int.Parse(projectLine[2]);
            project.Deadline = int.Parse(projectLine[3]);

            var rolesCount = int.Parse(projectLine[4]);

            for (int j = 0; j < rolesCount; j++)
            {
                var roleLine = lines[i + j + 1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var name = roleLine[0];
                var level = int.Parse(roleLine[1]);

                project.Roles.Add(name, level);
            }

            i += rolesCount + 1;

            projects.Add(project);
        }

        return new Dataset(contributors, projects);
    }
}