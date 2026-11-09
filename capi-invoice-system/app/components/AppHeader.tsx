type AppHeaderProps = {
  title: string;
  hasNotifications: boolean;
};

export function AppHeader({ title, hasNotifications }: AppHeaderProps) {
  return (
    <header className="topbar">
      <div className="breadcrumbs">
        <span>Workspace</span>
        <b>/</b>
        <strong>{title}</strong>
      </div>

      <div className="top-actions">
        <button className="icon-button" aria-label="Help">
          ?
        </button>
        <button className="icon-button bell" aria-label="Notifications">
          ♧{hasNotifications && <i />}
        </button>
        <div className="top-avatar">LM</div>
      </div>
    </header>
  );
}
