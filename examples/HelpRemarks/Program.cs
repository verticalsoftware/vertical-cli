// See https://aka.ms/new-console-template for more information

using HelpRemarks;
using Vertical.Cli;
using CliRemarks = Vertical.Cli.Help.HelpRemarks;

var command = new RootCommand<Options>(
    "Program",
    "Description of the program",
    remarks:
    [
        new CliRemarks("Remarks", 
        [
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
            "Molestie etiam tincidunt finibus iaculis eget vivamus. Leo quam sed potenti est accumsan diam. Congue porta leo adipiscing torquent fringilla ut. Dapibus nec pellentesque erat auctor enim quisque; venenatis mollis. Fermentum malesuada dolor duis placerat commodo pellentesque penatibus cursus. Praesent tristique penatibus habitant himenaeos hac lobortis?"
        ]),
        new CliRemarks("Description", 
        [
            "Ac fames leo; sapien pulvinar adipiscing penatibus. Sollicitudin venenatis parturient proin pulvinar aenean nisl sociosqu turpis. Nisl semper montes cursus tristique habitasse curae. Blandit consectetur rutrum praesent pulvinar ante maximus! Interdum viverra amet magna magna sagittis interdum. Ullamcorper vulputate augue quis eros gravida class torquent per.",
            "Ad erat accumsan accumsan; proin platea lobortis arcu erat. Elementum posuere enim sit cursus ut gravida dictum aliquam. Neque quam mattis dolor fermentum tempor platea ut felis. Rhoncus tortor sed viverra primis conubia ullamcorper fermentum orci ex. Sollicitudin auctor vivamus mattis platea nisi, porttitor sagittis himenaeos. Nibh urna varius metus erat imperdiet nostra."
        ])
    ]);

command.Handle(_ => 0);
command.AddHelpSwitch();

await command.InvokeAsync(args);    