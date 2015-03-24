using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Pcl
{
    public static class NavigationExtensions
    {

        async public static void PushAndRemovePrevious(this INavigation stack, Page page, int startAt)
        {
            var c = stack.NavigationStack.Count();

            await stack.PushAsync(page);

            for (int i = startAt; i < c; i++) {
                stack.RemovePage(stack.NavigationStack[startAt]);
            }
        }
    }
}

