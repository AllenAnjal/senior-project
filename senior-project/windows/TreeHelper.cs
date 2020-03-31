using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace senior_project
{
    public class TreeHelper
    {
        private readonly TreeView windowElement;
        private readonly string sectionTitle;
        private readonly string stepTitle;

        public TreeHelper(TreeView treeViewElement, string sectionName, string stepName)
        {
            windowElement = treeViewElement;
            sectionTitle = sectionName;
            stepTitle = stepName;
        }

        public void MoveForward()
        {
            MoveSelectedItem(true);
        }

        public void MoveBackwards()
        {
            MoveSelectedItem(false);
        }

        //If windowElement.SelectedItem has Name attribute of stepTitle
        //then set changed SelectedItem
        private bool MoveSelectedItem(bool moveForward)
        {
            int currSection, currStep, prevSection, prevStep, nextSection, nextStep;
            bool isFirst, isLast;
            var step = windowElement.SelectedItem as XmlNode;
            var section = step.ParentNode as XmlNode;

            var nodes = section.SelectNodes(stepTitle) as XmlNodeList;

            List<XmlNode> nList = new List<XmlNode>();
            foreach (XmlNode n in nodes) nList.Add(n);

            currStep = nList.FindIndex(i => i.Equals(step));
            isFirst = nList.First<XmlNode>().Equals(step);
            isLast = nList.Last<XmlNode>().Equals(step);

            nodes = section.ParentNode.SelectNodes(sectionTitle);

            nList.Clear();
            foreach (XmlNode n in nodes) nList.Add(n);

            currSection = nList.FindIndex(i => i.Equals(section));

            prevSection = currSection;
            prevStep = currStep - 1;
            nextSection = currSection;
            nextStep = currStep + 1;
            if (isFirst)
            {
                if (section.PreviousSibling == null)
                {
                    prevSection = 0;
                    prevStep = 0;
                }
                else
                {
                    prevSection = currSection - 1;
                    prevStep = section.PreviousSibling.SelectNodes(stepTitle).Count - 1;
                }
            }
            if (isLast)
            {
                if (section.NextSibling == null)
                {
                    nextStep = currStep;
                }
                else
                {
                    nextSection = currSection + 1;
                    nextStep = 0;
                }
            }

            TreeViewItem newSectionContainer = windowElement.ItemContainerGenerator.ContainerFromIndex(moveForward ? nextSection : prevSection) as TreeViewItem;
            TreeViewItem newStepContainer = newSectionContainer.ItemContainerGenerator.ContainerFromIndex(moveForward ? nextStep : prevStep) as TreeViewItem;
            newStepContainer.IsSelected = true;

            return true;
        }
    }
}