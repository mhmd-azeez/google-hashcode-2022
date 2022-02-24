using System.Text;

var names = new[] { "a_an_example", "b_better_start_small", "c_collaboration", "d_dense_schedule", "e_exceptional_skills", "f_find_great_mentors" };

foreach (var name in names)
{
    System.Console.WriteLine(name);
    Solve(name);
}

void Solve(string name)
{
    var dataset = Dataset.Parse($"input/{name}.in.txt");
    var submission = new Submission();

    foreach (var project in dataset.Projects
        .OrderByDescending(p => p.Score)
        .ThenBy(p => p.Duration)
        .ThenBy(p => p.Roles.Count))
    {
        var assignment = new ProjectAssignment();
        assignment.ProjectName = project.Name;
        var projectSatisfied = true;

        foreach (var role in project.Roles)
        {
            var contributor = dataset.Contributors
                .FirstOrDefault(c =>
                    c.Skills.Any(skill => skill.Key == role.Name && skill.Value >= role.Level) &&
                    assignment.Contributors.Contains(c) == false);

            if (contributor is null)
            {
                projectSatisfied = false;
                break;
            }

            dataset.Contributors.Remove(contributor);
            dataset.Contributors.Add(contributor);

            assignment.Contributors.Add(contributor);
            assignment.ContributorRole[contributor] = role.Name;
        }

        if (projectSatisfied)
        {
            submission.Projects.Add(assignment);

            foreach (var contributor in assignment.Contributors)
            {
                var role = project.Roles.First(r => r.Name == assignment.ContributorRole[contributor]);
                if (role.Level == contributor.Skills[role.Name])
                {
                    contributor.Skills[role.Name]++;
                }
            }
        }
    }

    submission.Write($"output/{name}.out.txt");
}

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
    public List<Role> Roles { get; set; } = new();

}

public class Role
{
    public int Level { get; set; }
    public string Name { get; set; }
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

                project.Roles.Add(new Role { Name = name, Level = level });
            }

            i += rolesCount + 1;

            projects.Add(project);
        }

        return new Dataset(contributors, projects);
    }
}

public class Submission
{
    public List<ProjectAssignment> Projects { get; set; } = new();

    public void Write(string path)
    {
        var builder = new StringBuilder();

        builder.AppendLine(Projects.Count.ToString());

        foreach (var project in Projects)
        {
            builder.AppendLine(project.ProjectName);
            builder.AppendLine(string.Join(' ', project.Contributors.Select(c => c.Name)));
        }

        File.WriteAllText(path, builder.ToString());
    }
}

public class ProjectAssignment
{
    public string ProjectName { get; set; }
    public Dictionary<Contributor, string> ContributorRole { get; set; } = new();
    public HashSet<Contributor> Contributors { get; set; } = new();
}