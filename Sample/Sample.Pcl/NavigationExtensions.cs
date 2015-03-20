using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Pcl
{
    public static class NavigationExtensions
    {

        public static bool isPushing;

        async public static void PushAndRemovePrevious(this INavigation stack, Page page, int startAt)
        {
            var c = stack.NavigationStack.Count();

            await stack.PushAsync(page);

            for (int i = startAt; i < c; i++) {

                stack.RemovePage(stack.NavigationStack[startAt]);
            }
        }

        async public static void PopToRootAndPush(this INavigation stack, Page page)
        {
            await stack.PopToRootAsync();
            await stack.PushAsync(page);
        }

        async public static Task BlockingPushAsync(this INavigation stack, Page page, bool animated = true)
        {
            if (!isPushing) {
                isPushing = true;
                await stack.PushAsync(page, animated);
                isPushing = false;
            }
        } 
    }
}

