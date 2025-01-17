export interface FeatureItemProps extends React.HTMLAttributes<HTMLDivElement> {
  icon: JSX.Element;
  title: string;
  description: string;
}

const FeatureItem = ({ icon, title, description, className, ...rest }: FeatureItemProps) => {
  return (
    <div className={`${className}`} {...rest}>
      <div className="flex gap-2 items-center">
        {icon}
        <p className="text-4xl font-light">{title}</p>
      </div>
      <p className="font-light">{description}</p>
    </div>
  );
};

export default FeatureItem;