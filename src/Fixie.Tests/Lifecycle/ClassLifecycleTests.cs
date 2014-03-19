using System;

namespace Fixie.Tests.Lifecycle
{
    public class ClassLifecycleTests : LifecycleTests
    {
        public void ShouldAllowWrappingTypeWithBehaviorsWhenConstructingPerCase()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Inner Before");
                          innerBehavior();
                          Console.WriteLine("Inner After");
                      })
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Outer Before");
                          innerBehavior();
                          Console.WriteLine("Outer After");
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "Outer Before", "Inner Before",
                ".ctor", "Pass", "Dispose",
                ".ctor", "Fail", "Dispose",
                "Inner After", "Outer After");
        }

        public void ShouldAllowWrappingTypeWithBehaviorsWhenConstructingPerTestClass()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Inner Before");
                          innerBehavior();
                          Console.WriteLine("Inner After");
                      })
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Outer Before");
                          innerBehavior();
                          Console.WriteLine("Outer After");
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "Outer Before", "Inner Before",
                ".ctor", "Pass", "Fail", "Dispose",
                "Inner After", "Outer After");
        }

        public void ShouldAllowClassBehaviorsToShortCircuitInnerBehaviorWhenConstructingPerCase()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          //Behavior chooses not to invoke innerBehavior().
                          //Since the test classes are never intantiated,
                          //their cases don't have the chance to throw exceptions,
                          //resulting in all 'passing'.
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail passed.");

            output.ShouldHaveLifecycle();
        }

        public void ShouldAllowClassBehaviorsToShortCircuitInnerBehaviorWhenConstructingPerTestClass()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          //Behavior chooses not to invoke innerBehavior().
                          //Since the test classes are never intantiated,
                          //their cases don't have the chance to throw exceptions,
                          //resulting in all 'passing'.
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail passed.");

            output.ShouldHaveLifecycle();
        }

        public void ShouldFailCaseWhenConstructingPerCaseAndClassBehaviorThrows()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Unsafe class execution behavior");
                          throw new Exception("Unsafe class execution behavior threw!");
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: Unsafe class execution behavior threw!",
                "SampleTestClass.Fail failed: Unsafe class execution behavior threw!");

            output.ShouldHaveLifecycle("Unsafe class execution behavior");
        }

        public void ShouldFailAllCasesWhenConstructingPerTestClassAndClassBehaviorThrows()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Unsafe class execution behavior");
                          throw new Exception("Unsafe class execution behavior threw!");
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: Unsafe class execution behavior threw!",
                "SampleTestClass.Fail failed: Unsafe class execution behavior threw!");

            output.ShouldHaveLifecycle("Unsafe class execution behavior");
        }

        public void ShouldFailCaseWithOriginalExceptionWhenConstructingPerCaseAndClassBehaviorThrowsPreservedException()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Unsafe class execution behavior");
                          try
                          {
                              throw new Exception("Unsafe class execution behavior threw!");
                          }
                          catch (Exception originalException)
                          {
                              throw new PreservedException(originalException);
                          }
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: Unsafe class execution behavior threw!",
                "SampleTestClass.Fail failed: Unsafe class execution behavior threw!");

            output.ShouldHaveLifecycle("Unsafe class execution behavior");
        }

        public void ShouldFailAllCasesWithOriginalExceptionWhenConstructingPerTestClassAndClassBehaviorThrowsPreservedException()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .Wrap((classExecution, innerBehavior) =>
                      {
                          Console.WriteLine("Unsafe class execution behavior");
                          try
                          {
                              throw new Exception("Unsafe class execution behavior threw!");
                          }
                          catch (Exception originalException)
                          {
                              throw new PreservedException(originalException);
                          }
                      });

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: Unsafe class execution behavior threw!",
                "SampleTestClass.Fail failed: Unsafe class execution behavior threw!");

            output.ShouldHaveLifecycle("Unsafe class execution behavior");
        }

        public void ShouldAllowWrappingTypeWithSetUpTearDownBehaviorsWhenConstructingPerCase()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Dispose",
                ".ctor", "Fail", "Dispose",
                "TypeTearDown");
        }

        public void ShouldAllowWrappingTypeWithSetUpTearDownBehaviorsWhenConstructingPerTestClass()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Fail", "Dispose",
                "TypeTearDown");
        }

        public void ShouldShortCircuitInnerBehaviorAndTearDownByFailingCaseWhenConstructingPerCaseAndTypeSetUpThrows()
        {
            FailDuring("TypeSetUp");

            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeSetUp' failed!",
                "SampleTestClass.Fail failed: 'TypeSetUp' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp");
        }

        public void ShouldShortCircuitInnerBehaviorAndTearDownByFailingAllCasesWhenConstructingPerTestClassAndTypeSetUpThrows()
        {
            FailDuring("TypeSetUp");

            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeSetUp' failed!",
                "SampleTestClass.Fail failed: 'TypeSetUp' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp");
        }

        public void ShouldFailCaseWhenConstructingPerCaseAndTypeTearDownThrows()
        {
            FailDuring("TypeTearDown");

            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeTearDown' failed!",
                "SampleTestClass.Fail failed: 'Fail' failed!" + Environment.NewLine +
                "    Secondary Failure: 'TypeTearDown' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Dispose",
                ".ctor", "Fail", "Dispose",
                "TypeTearDown");
        }

        public void ShouldFailAllCasesWhenConstructingPerTestClassAndTypeTearDownThrows()
        {
            FailDuring("TypeTearDown");

            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .SetUpTearDown(TypeSetUp, TypeTearDown);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeTearDown' failed!",
                "SampleTestClass.Fail failed: 'Fail' failed!" + Environment.NewLine +
                "    Secondary Failure: 'TypeTearDown' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Fail", "Dispose",
                "TypeTearDown");
        }

        public void ShouldAllowWrappingTypeWithSetUpBehaviorWhenConstructingPerCase()
        {
            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .SetUp(TypeSetUp);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Dispose",
                ".ctor", "Fail", "Dispose");
        }

        public void ShouldAllowWrappingTypeWithSetUpBehaviorWhenConstructingPerTestClass()
        {
            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .SetUp(TypeSetUp);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass passed.",
                "SampleTestClass.Fail failed: 'Fail' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp",
                ".ctor", "Pass", "Fail", "Dispose");
        }

        public void ShouldShortCircuitInnerBehaviorByFailingCaseWhenConstructingPerCaseAndTypeSetUpThrows()
        {
            FailDuring("TypeSetUp");

            Convention.ClassExecution
                      .CreateInstancePerCase()
                      .SetUp(TypeSetUp);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeSetUp' failed!",
                "SampleTestClass.Fail failed: 'TypeSetUp' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp");
        }

        public void ShouldShortCircuitInnerBehaviorByFailingAllCasesWhenConstructingPerTestClassAndTypeSetUpThrows()
        {
            FailDuring("TypeSetUp");

            Convention.ClassExecution
                      .CreateInstancePerTestClass()
                      .SetUp(TypeSetUp);

            var output = Run();

            output.ShouldHaveResults(
                "SampleTestClass.Pass failed: 'TypeSetUp' failed!",
                "SampleTestClass.Fail failed: 'TypeSetUp' failed!");

            output.ShouldHaveLifecycle(
                "TypeSetUp");
        }
    }
}